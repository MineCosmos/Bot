using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Kook.WebSocket;
using MineCosmos.Bot.Common;
using MineCosmos.Bot.Entity;
using MineCosmos.Bot.Service;
using MineCosmos.Bot.Service.Bot;
using Minecraft.Messages;
using Newtonsoft.Json;
using SqlSugar;

namespace MineCosmos.Bot.Service;
    /// <summary>
    /// 公共指令
    /// </summary>
    public class BaseCommand:BaseService
    {
        public BaseCommand() { }

        //这里由子类赋值
        public CommandData _data;
        public readonly SqlSugarScope Db = SqlSugarHelper.Instance;

        public ulong KookUserId => _data.SocketGuildUser.Id;

        public List<BtnClickEventModel> btnClickEventModels = new()
        {
             new (){
                 Text = "自杀",
                 Value ="killself",
                 Desc = "把在服务器里的你处死"
             },
             new (){ Text = "传送", Value = "tp", Desc = "向对方发起传送，或者邀请对方传送"},
             new (){ Text = "当前频道服务器信息", Value = "info", Desc = "展示相关信息"},
        };


     

        #region 业务仓储

        public async Task<PlayerInfoEntity> GetPlayerInfoByKookIdAsync(string kookId) => await GetAsync< PlayerInfoEntity>(a => a.KookUserId == kookId);

        public async Task<bool> AnyPlayerInfoByKookId(string kookId) => await AnyAsync<PlayerInfoEntity>(a => a.KookUserId == kookId);

        #endregion



        /// <summary>
        /// 当前MC服务器，子类构造中赋值
        /// </summary>
        public MinecraftServerEntity McServer { get; set; }

        /// <summary>
        /// 获取当前kook对应的mc服务器
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<MinecraftServerEntity> GetCurrentMcServer()
        {
            if (_data.SocketGuildUser is null) throw new Exception("当前kook对象为空，无法进行获取MC服务器名称操作");
            string serverId = _data.SocketGuildUser.Guild.Id.ToString();

            bool has = await AnyAsync<MinecraftServerEntity>(a => a.KookGuild == serverId);
            if (!has) throw new Exception("当前Kook服务器还未MC服务器");

            McServer = await GetAsync<MinecraftServerEntity>(a => a.KookGuild == serverId);

            return McServer;
        }

        /// <summary>
        /// 获取字符串分割后的指令数组
        /// *如果指令大于1个，则会跳过第一位
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string[] GetCommandArry()
        {
            if (_data.SocketMessage is null) throw new Exception("SockerMessage是空的");

            string[] parts =
                _data.SocketMessage.Content
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 1) parts = parts.Skip(1).ToArray();

            return parts;
        }

        #region 验证码

        /// <summary>
        /// 验证码的rediskey值，0 代表操作类型  1 代表kook的id 2 是验证码
        /// </summary>
        private string CodeRedisKey = "mc:code:{0}:{1}:{2}";

        /// <summary>
        /// 发送验证码
        /// </summary>
        public async Task SendCodeToMinecraftServer(string mcPlayerName)
        {
            if (_data.ServerManager is null) throw new Exception("缺少服务器管理服务");
            if (_data.SocketGuildUser is null) throw new Exception("缺少kook服务器用户服务");

            var code = new Random().Next(11111, 99999);
            var rediskey = string.Format(CodeRedisKey, (int)VerificationCodeEnum.绑定账号, _data.SocketGuildUser.Id, code);
            if (RedisHelper.Exists(rediskey)) RedisHelper.Del(rediskey);

            await SendVcodeMsgToMc(code, mcPlayerName);

            var codeData = new VerificationCodeModel
            {
                Code = code,
                PlayerName = mcPlayerName,
                Type = (int)VerificationCodeEnum.绑定账号
            };

            await RedisHelper.SetAsync(rediskey, codeData.ToJson(), 60 * 1);
        }

        /// <summary>
        /// 获取验证码数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<VerificationCodeModel?> GetCodeDataAsync(VerificationCodeEnum type, string code)
        {
            if (_data.SocketGuildUser is null) throw new Exception("缺少kook服务器玩家服务");

            var rediskey = string.Format(CodeRedisKey, (int)type, _data.SocketGuildUser.Id, code);

            if (!RedisHelper.Exists(rediskey)) return null;

            return await RedisHelper.GetAsync<VerificationCodeModel>(rediskey);
        }

        private async Task SendVcodeMsgToMc(int vcode, string mcName, string operName = "kook绑定操作")
        {
            if (_data.ServerManager is null) throw new Exception("缺少服务器管理服务");

            MessageComponent titleCcomponent = MessageComponent.Create()
                     .WithText($"[KOOK] ")
                     .WithColor(MinecraftColor.Green)
                     .WithFont("minecraft:default")
                     .WithClickEvent(ClickAction.OpenUrl, "https://www.kookapp.cn/app/invite/CQ0ojJ")
                     .WithHoverEvent(HoverAction.ShowText, $"[Kook频道] [点击加入]");

            MessageComponent senderCcomponent = MessageComponent.Create()
                    .WithText($"[{DateTime.Now.ToString("HH:mm")}] 你正在进行{operName},你的验证码是：")
                    .WithColor(MinecraftColor.Green)
                    .WithFont("minecraft:default");

            MessageComponent contentCcomponent = MessageComponent.Create()
                    .WithText($" {vcode}")
                    .WithColor(MinecraftColor.Red)
                    .WithFont("minecraft:default");

            JsonMessageBuilder builder1 = new();
            builder1.AddComponent(titleCcomponent)
                    .AddComponent(senderCcomponent)
                    .AddComponent(contentCcomponent);
            string json = builder1.ToJson();
            string mcCommand = $"tellraw {mcName} {json}";
            var sayRes = await _data.ServerManager!.SendAsync(_data.CurrentMcServer.Id, mcCommand);
        }

        #endregion

        #region 指令操作统一返回值


        public CommandExcuteResultModel Success(string message) => new CommandExcuteResultModel { IsReplyMsg = true, Message = message, Success = true };
        public CommandExcuteResultModel Success() => new CommandExcuteResultModel { IsReplyMsg = false, Success = true };

        public CommandExcuteResultModel Error(string message) => new CommandExcuteResultModel { IsReplyMsg = true, Message = message, Success = false };
        #endregion
    }

    /// <summary>
    /// redis中存放的验证码数据模型
    /// </summary>
    public class VerificationCodeModel
    {
        /// <summary>
        /// MC服务器玩家名称
        /// *因为是服务器内接收验证码，所以不管什么验证码这个值肯定会存在
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// 验证码本尊
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 验证码类型
        /// </summary>
        public int Type { get; set; }
    }
    public enum VerificationCodeEnum
    {
        绑定账号 = 3001
    }

    public class BtnClickEventModel
    {
        public string Value { get; set; }
        public string Desc { get; set; }
        public string Text { get; set; }
    }

    public class CommandExcuteResultModel
    {
        public bool Success { get; set; }
        /// <summary>
        /// 是否需要统一回复消息
        /// </summary>
        public bool IsReplyMsg { get; set; }
        public string Message { get; set; }
    }

