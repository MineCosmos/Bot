using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CoreRCON;
using Mapster.Models;
using MineCosmos.Bot.Entity;
using MineCosmos.Bot.Entity.Dto;
using MineCosmos.Bot.Service.Common;
using SqlSugar;

namespace MineCosmos.Bot.Service;

/// <summary>
/// 玩家服务
/// </summary>
public class PlayerService : BaseService, IPlayerService
{
    readonly ICommonService _commonService;

    public PlayerService(ICommonService commonService)
    {
        _commonService = commonService;
    }
    /// <summary>
    ///  保存玩家信息
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<bool> SavePlayer(PlayerInfoEntity model)
    {
        bool hasVal = await AnyAsync<PlayerInfoEntity>(
            a => a.Uuid == model.Uuid && (model.Id < 0 || a.Id != model.Id));
        //TODO 封装异常
        if (hasVal) throw new Exception("已存在相同uuid玩家");
        return await Save<PlayerInfoEntity>(model);
    }

    /// <summary>
    /// 修改玩家头像
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public async Task UpdatePlayerAvatar(int playerId,string newAvatarUrl) => await UpdateColumnsAsync<PlayerInfoEntity>(a => a.Id == playerId, c => c.Avatar == newAvatarUrl);

    /// <summary>
    /// 获取玩家分页列表
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<ResPageDto<PlayerInfoEntity>> PagePlayer(ReqPageDto model) => await GetIncludesPageDataAsync<PlayerInfoEntity>(model, a => model.Key == "" || a.Name.Contains(model.Key));

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
}



