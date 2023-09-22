using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MineCosmos.Bot.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Flurl.Http;
using SqlSugar;
using System.Diagnostics.Metrics;
using System.Diagnostics.Tracing;
using MineCosmos.Bot.Service.Common;

namespace MineCosmos.Bot.Service.Bot
{
    #region MyRegion

    public class CommandModel
    {
        /// <summary>
        /// 指令列表，固定第一位是指令名称即： ！xxx  的xxx
        /// </summary>
        public string[] Commands { get; set; }
        public bool Success { get; set; }
        public string ErroMessage { get; set; }
    }

    #endregion

    public class BaseBotService : BaseService, IBotService
    {
        readonly ICommonService _commonService;
        public BaseBotService(ICommonService commonService)
        {
            _commonService = commonService;
        }

        /// <summary>
        /// 签到
        /// </summary>
        /// <returns></returns>
        public async Task<Stream?> SingInByPlayerId(int playerId)
        {
            var playerInfo = await _db.Queryable<PlayerInfoEntity>().FirstAsync(a => a.Id == playerId);
            if (playerInfo is null) return _commonService.GenerateImageToStream("签到失败,找不到玩家信息");

            var signInInfo = await _db.Queryable<PlayerSingInRecordEntity>()
           .Where(a => a.PlayerId == playerId && a.CreateTime == DateTime.Now.ToString("yyyy-MM-dd"))
             .OrderByDescending(a => a.CreateTime)
         .FirstAsync();

            if (signInInfo == null)
            {
                var recordNum = await SqlSugarHelper.Instance.Queryable<PlayerSingInRecordEntity>()
               .Where(a => a.Id == playerId)
               .CountAsync();

                int signInCount = recordNum == 0 ? 1 : recordNum;
                int emeraldVal = new Random().Next(1, signInCount);
                var integral = new Random().Next(1, 3);
                var luckColors = new List<string> { "红色", "黄色", "绿色", "蓝色", "紫色" };
                var luckColorndex = new Random().Next(0, luckColors.Count - 1);
                var luckNumber = new Random().Next(111, 999);
                var luckVal = new Random().Next(1, 100);

                await _db.Insertable(new PlayerSingInRecordEntity
                {
                    PlayerId = playerInfo.Id,
                    CreateUserId = playerInfo.Id,
                    UpdateUserId = playerInfo.Id,
                    Integral = integral,
                    EmeraldVal = emeraldVal,
                    LuckColor = luckColors[luckColorndex],
                    LuckNumber = luckNumber,
                    LuckVal = luckVal,
                }).ExecuteCommandAsync();



                playerInfo.SignInCount += signInCount;
                playerInfo.EmeraldVal += emeraldVal;
                playerInfo.UpdateUserId = playerInfo.Id;
                await SqlSugarHelper.Instance.Updateable(playerInfo).ExecuteCommandAsync();

                return _commonService.GenerateImageToStream("签到失败,找不到玩家信息");
            }
            return null;
        }

        /// <summary>
        /// 创建玩家
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<PlayerInfoEntity> CreateNewPlayer(PlayerInfoEntity entity)
        {
            var result = _db.InsertNav(entity)
         .Include(it => it.ListPlayerPlatformEntity, new InsertNavOptions()
         { OneToManyIfExistsNoInsert = true })//配置存在不插入
         .ExecuteCommand();//用例代码

            return await _db.Insertable(entity).ExecuteReturnEntityAsync();
        }

        /// <summary>
        /// 根据平台ID获取玩家信息
        /// </summary>
        /// <param name="platformId"></param>
        /// <returns></returns>
        public async Task<PlayerInfoEntity> GetPlayerInfoByPlatformId(string platformId)
        {
            var platFormInfo = await _db.Queryable<PlayerPlatformEntity>().FirstAsync(a => a.PlatformId == platformId);
            if (platFormInfo == null)
            {
                return null;
            }
            return await _db.Queryable<PlayerInfoEntity>().FirstAsync(a => a.Id == platFormInfo.PlayerId);
        }


        /// <summary>
        /// 根据平台上的名称（QQ号获取玩家信息
        /// </summary>
        /// <param name="platformId"></param>
        /// <returns></returns>
        public async Task<PlayerInfoEntity> GetPlayerInfoByPlatformName(string platformName)
        {
            var platFormInfo = await _db.Queryable<PlayerPlatformEntity>().FirstAsync(a => a.PlatformName == platformName);
            if (platFormInfo == null)
            {
                return null;
            }
            return await _db.Queryable<PlayerInfoEntity>().FirstAsync(a => a.Id == platFormInfo.PlayerId);
        }

        /// <summary>
        /// 发言记录
        /// </summary>
        /// <param name="platformId">QQ,KOOK,discord 其中的用户标识，例如qq就是qq</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<Stream?> SpeechRecordByPlayerId(string platformId, string platformName, string message)
        {
            var avator = await _commonService.DownloadQQAvator(platformId);

            var platFormInfo = await _db.Queryable<PlayerPlatformEntity>().FirstAsync(a => a.PlatformId == platformId);
            PlayerInfoEntity playerInfoEntity = new()
            {
                Avatar = avator,
                EmeraldVal = 100,
                Name = platformName
            };
            if (platFormInfo == null)
            {
                playerInfoEntity.ListPlayerPlatformEntity = new() {
                    new() {PlatformId = platformId, PlatformName = platformName }
                };
                var playerInfo = await CreateNewPlayer(playerInfoEntity);
                var imgStream = _commonService.GenerateImageToStream("第一次发言 o.O \r\n 送你100个绿宝石", 10);
                return imgStream;
            }
            else
            {
                var playerInfo = await _db.Queryable<PlayerInfoEntity>().FirstAsync(a => a.Id == platFormInfo.PlayerId);
                if (playerInfo == null) await _db.Insertable(playerInfoEntity).ExecuteCommandAsync();
                else
                {
                    playerInfo.EmeraldVal += 1;
                    playerInfo.UpdateUserId = playerInfo.Id;
                    _db.Updateable(playerInfoEntity);
                }
                return null;
            }
        }

        /// <summary>
        /// 从接收到的消息中获取指令
        /// </summary>
        /// <param name="msgText"></param>
        /// <returns></returns>
        public async Task<CommandModel> GetCommandFromMessage(string msgText)
        {
            CommandModel commandInfo = new() { Success = false, ErroMessage = "获取失败" };


            if (string.IsNullOrWhiteSpace(msgText))
                return new() { ErroMessage = "消息为空", Success = false };

            int position = Math.Max(msgText.IndexOf("！"), msgText.IndexOf("!"));
            if (position < 0)
                return new() { ErroMessage = "不是有效的指令", Success = false };

            msgText = msgText
              .Replace("\r\n", " ")
              .Replace("\r", " ")
              .Replace("\n", " ")
            .Replace("!", string.Empty)
              .Replace("！", string.Empty);
            msgText = Regex.Replace(msgText, @"\s+", replacement: " ");

            return new() { Commands = msgText.Split(' '), Success = true };


            // commandInfo.Commands msgText.Split(' ');
        }


    }
}
