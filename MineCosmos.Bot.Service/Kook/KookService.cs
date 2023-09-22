using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Kook;
using Kook.Rest;
using Kook.WebSocket;
using MineCosmos.Bot.Common;
using MineCosmos.Bot.Entity;
using MineCosmos.Bot.Service;
using MineCosmos.Bot.Service.Bot;
using MineCosmos.Bot.Service.Common;
using Minecraft.Messages;
using Serilog.Events;

namespace MineCosmos.Bot.Service;
public class KookService : IKookService
{

    public ICommonService? _commonService;
    public IServerManagerService? _serverManagerService;
    public static KookSocketClient? _client;
    public KookService(IServerManagerService serverManagerService, ICommonService commonService)
    {
        _serverManagerService = serverManagerService;
        _commonService = commonService;
    }

    public async Task StartKookNet()
    {
        try
        {
            _client = new KookSocketClient();
            var token = AppSettings.app(new string[] { "Kook", "Token" });
            await _client.LoginAsync(TokenType.Bot, token);

            _client.Ready += () =>
            {
                "神机自动作战机甲KOOK形态已就绪".PrintSuccess();

                StringBuilder sb = new();
                foreach (var item in _client.Guilds)
                {
                    sb.AppendLine(item.Name);
                }

                //ABC服务器 - 测试
                _ = _client.GetGuild(6079132046041885)
                .GetTextChannel(4154290969579404)
                .SendCardAsync("自动作战机甲已启动 · 瓦达西瓦加载完了思密达".Colorize(TextTheme.Success).KookSuccessCard());

                return Task.CompletedTask;
            };
            _client.Connected += () =>
            {
                "神机自动作战机甲已连接至KOOK".PrintSuccess();
                return Task.CompletedTask;
            };
            _client.Disconnected += (e) =>
            {
                "神机自动作战机甲KOOK作战形态崩坏".PrintError();
                return Task.CompletedTask;
            };
            _client.MessageUpdated += MessageUpdated; ;
            _client.MessageReceived += MessageReceived;
            _client.MessageButtonClicked += async (value, user, message, channel) =>
            {

            };
            _client.Log += LogAsync;
            await _client.StartAsync();
        }
        catch (Exception ex)
        {
            ex.Message.PrintError();
            await _client!.StopAsync();
            _client.Dispose();
            await StartKookNet();
        }
    }

    private async Task LogAsync(LogMessage message)
    {
        var severity = message.Severity switch
        {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Info => LogEventLevel.Information,
            // Serilog 中，LogEventLevel.Verbose 相比 LogEventLevel.Debug 会输出更多的信息
            LogSeverity.Verbose => LogEventLevel.Debug,
            LogSeverity.Debug => LogEventLevel.Verbose,
            _ => LogEventLevel.Information
        };

        Serilog.Log.Write(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);

        await Task.CompletedTask;
    }

    /// <summary>
    /// 收到消息事件
    /// </summary>
    /// <param name="message"></param>
    /// <param name="guildUser"></param>
    /// <param name="textChannel"></param>
    /// <returns></returns>
    private async Task MessageReceived(SocketMessage message, SocketGuildUser guildUser, SocketTextChannel textChannel)
    {
        if (message.Author.IsBot!.Value) return;
        #region 记录
        //ConsoleHelper.LogMessage("Kook收到消息事件", new string[] {
        //    $"频道名称： {textChannel.Name}",
        //    $"发言人：{guildUser.Nickname}/{guildUser.DisplayName}",
        //    $"原始消息：{message.RawContent}",
        //    $"消息ID：{message.Id}",
        //    $"消息类型：{message.GetType().Name}",
        //    $"消息内容：{message.Content}",
        //    $"消息来源：{message.Source}",
        //    $"消息时间戳：{message.Timestamp}"
        //});
        Console.WriteLine($"{textChannel.Name} {guildUser.Nickname}/{guildUser.DisplayName} {message.Content}");
        #endregion



        //限定频道
        string codeChannel = AppSettings.app(new string[] { "Kook", "CodeChannel" });
        if (textChannel.Name != codeChannel) return;





        try
        {
            //指令处理
            var codeArry = message.Content.Split(" ").ToArray();
            await message.AddReactionAsync(new Kook.Emoji("⌛"));
            var erroMessage = await HandleCode(codeArry, guildUser, message);
            if (erroMessage.IsNotEmptyOrNull()) await message.ExecuteKookError(erroMessage);
        }
        catch (Exception ex)
        {
            await textChannel.SendTextAsync($"程序出现异常，请发送这个截图给CTU,错误信息： {ex.Message}");
        }


    }

    public async Task<string> HandleCode(string[] codeArry, SocketGuildUser socketGuildUser, SocketMessage message)
    {
        if (codeArry.Length < 1)
        {
            await message.ExecuteKookError();
            return "指令长度无效";
        }

        string startCommand = codeArry[0];


        #region 未绑定时可以做的操作
        if (codeArry[0].ToString() == "bind" || codeArry[0].ToString() == "绑定")
        {
            if (codeArry.Length < 2) return "绑定指令格式不正确，示例:bind 名字";
            if (codeArry[1] == null) return "绑定指令格式不正确，示例:bind 名字";
            var vCode = new Random().Next(11111, 99999);
            var mcName = codeArry[1].ToString();
            //数据库先判断玩家信息有效性
            PlayerInfoEntity? dbUserInfo = await SqlSugarHelper.Instance.Queryable<PlayerInfoEntity>()
                .FirstAsync(expression: a => a.Name == mcName);

            //禁止重复绑定
            //if (dbUserInfo != null && dbUserInfo!.KookUserId!.IsNotEmptyOrNull()) return "你已经绑定过了，请愉快的享用功能";

            //mc服务器校验
            var checkOnline = await _serverManagerService!.SendAsync("ABC", $"kook checkonline {mcName}");
            if (checkOnline == "0" || checkOnline.IsNullOrEmpty()) return "此功能需要玩家在服务器，你并没有在线，请登录游戏后重试";

            var rediskey = $"bind:vode:{vCode}:{socketGuildUser.Id}";
            if (RedisHelper.Exists(rediskey)) RedisHelper.Del(rediskey);
            //发送验证码
            await SendVcodeMsgToMc(vCode, mcName);
            await RedisHelper.SetAsync(rediskey, mcName, 60 * 1);

            if (dbUserInfo != null && dbUserInfo!.KookUserId!.IsNotEmptyOrNull())
            {
                await message.ExecuteKookSuccess($"你已经绑定过{dbUserInfo.Name},此举将会更换绑定，验证码发送成功，请在游戏内查收,");
            }
            else
            {
                await message.ExecuteKookSuccess("验证码发送成功，请在游戏内查收");
            }


            return string.Empty;
        }

        if (codeArry[0].ToString() == "验证码" || codeArry[0].ToString() == "vcoe")
        {
            if (codeArry.Length < 2) return "验证码校验指令格式不正确，示例:vcode 你在游戏内收到的验证码";
            if (codeArry[1] == null) return "验证码校验指令格式不正确，示例:vcode 你在游戏内收到的验证码";
            var reviceCode = codeArry[1].ToString();
            string errorMsg = await CheckVcode(reviceCode, socketGuildUser.Id.ToString());
            if (errorMsg.IsNullOrEmpty())
            {
                await message.ExecuteKookSuccess("绑定成功");
            }
        }

        //验证码特殊处理
        Regex regex = new Regex(@"^\d{5}$");
        if (regex.IsMatch(codeArry[0].ToString()))
        {
            var emsg = await CheckVcode(codeArry[0].ToString(), socketGuildUser.Id.ToString());
            if (emsg.IsNullOrEmpty())
            {
                await message.ExecuteKookSuccess("绑定成功");
            }
            else
            {
                await message.ExecuteKookError(emsg);
            }
            return string.Empty;
        }


        #endregion


        if (!await AnyBindKookToMc(socketGuildUser.Id.ToString()))
        {
            await message.ExecuteKookError("你还没有绑定kook，请输入：bind 你的游戏ID 进行绑定操作");
            return string.Empty;
        }

        var playerInfo = await GetPlayerInfoByKookId(socketGuildUser.Id.ToString());


        switch (codeArry[0])
        {
            case "小号过来":
                await _serverManagerService!.SendAsync("ABC", "execute as GreatMingNing run tp  GreatMingCtu");
                await message.ExecuteKookSuccess();
                break;
            case "去小号那":
                await _serverManagerService!.SendAsync("ABC", "execute as GreatMingCtu run tp  GreatMingNing");
                await message.ExecuteKookSuccess();
                break;
            case "tp":
            case "传送":
                if (codeArry.Length != 2)
                {
                    return "tp 指令格式不正确，示例： tp player";
                }
                var p1 = codeArry[1].ToString();
                var p2 = playerInfo.Name;
                var res = await _serverManagerService!.SendAsync("ABC", $"tpa {p1} {p2}").ReplaceMcTextToKook();


                await message.ExecuteKookSuccess(res);
                break;
            case "我在哪":
            case "换绑":

                if (codeArry.Length != 2)
                {
                    return "tp 指令格式不正确，示例： 换绑 新的游戏ID名称";
                }
                var newPlayerName = codeArry[1].ToString();
                var oriPlayerName = playerInfo.Name;

                await message.Channel.SendTextAsync($"", Quote.Empty, message.Author);


                string jsonC = $@"
[
  {{
    ""type"": ""card"",
    ""theme"": ""secondary"",
    ""size"": ""lg"",
    ""modules"": [
      {{
        ""type"": ""header"",
        ""text"": {{
          ""type"": ""plain-text"",
          ""content"": ""你正在进行换绑操作！{oriPlayerName} 将变更为 {newPlayerName}""
        }}
      }},
      {{
        ""type"": ""action-group"",
        ""elements"": [
          {{
            ""type"": ""button"",
            ""theme"": ""primary"",
            ""value"": ""changeAccount"",
            ""text"": {{
              ""type"": ""plain-text"",
              ""click"": ""return-val"",
              ""content"": ""确认变更""
            }}
          }},
          {{
            ""type"": ""button"",
            ""theme"": ""danger"",
            ""value"": ""cancel"",
            ""text"": {{
              ""type"": ""plain-text"",
              ""content"": ""取消操作""
            }}
          }}
        ]
      }},
      {{
        ""type"": ""divider""
      }},
      {{
        ""type"": ""context"",
        ""elements"": [
          {{
            ""type"": ""plain-text"",
            ""content"": ""MineCosmos.Kook 机器人V1""
          }}
        ]
      }}
    ]
  }}
]";


                //await message.Channel.SendCardAsync(
                //    CardJsonExtension.ParseMany(jsonC).FirstOrDefault()!.Build(), null, message.Author);



                break;
            case "自杀":
                await _serverManagerService!.SendAsync("ABC", $"kill {playerInfo.Name}");
                await message.ExecuteKookSuccess("自杀成功");
                break;
            case "say":

                if (codeArry.Length < 2) return "绑定指令格式不正确，示例:say 内容";
                if (codeArry[1] == null) return "绑定指令格式不正确，示例:say 内容";

                var sayUser = await SqlSugarHelper.Instance.Queryable<PlayerInfoEntity>().FirstAsync(a => a.KookUserId == socketGuildUser.Id.ToString());
                if (sayUser == null) return "找不到你的数据，请登录服务器游玩后进行绑定";


                //string tellrawMessage = $@"{{""text"":""{sayUser.Name}:{codeArry[1].ToString()}"",""color"":""green"",""clickEvent"":{{""action"":""open_url"",""value"":""https://kook.top/RWEeDu""}}}}";

                string onlineText = "离线";
                if (await CheckOnline(sayUser.Name)) onlineText = "在线";

                MessageComponent titleCcomponent = MessageComponent.Create()
                .WithText($"[KOOK] ")
                .WithColor(MinecraftColor.Green)
                .WithFont("minecraft:default")
                .WithClickEvent(ClickAction.OpenUrl, "https://www.kookapp.cn/app/invite/CQ0ojJ")
                .WithHoverEvent(HoverAction.ShowText, $"[{message.Channel.Name}频道] [点击加入]");

                MessageComponent statusCcomponent = MessageComponent.Create()
                      .WithText($"[{onlineText}]")
                      .WithColor(MinecraftColor.Gray)
                      .WithFont("minecraft:default");

                MessageComponent senderCcomponent = MessageComponent.Create()
                        .WithText($"[{message.Timestamp.ToString("HH:mm")}] <{sayUser.Name}>")
                        .WithColor(MinecraftColor.White)
                        .WithFont("minecraft:default")
                        .WithHoverEvent(HoverAction.ShowText, $"{sayUser.Name}");

                MessageComponent contentCcomponent = MessageComponent.Create()
                        .WithText($" {codeArry[1].ToString()}")
                        .WithColor(MinecraftColor.White)
                        .WithFont("minecraft:default")
                        .WithHoverEvent(HoverAction.ShowText, $"想要回复消息？来KOOK频道！");
                JsonMessageBuilder builder1 = new();
                builder1.AddComponent(titleCcomponent)
                        .AddComponent(statusCcomponent)
                        .AddComponent(senderCcomponent)
                        .AddComponent(contentCcomponent);
                string json = builder1.ToJson();
                string mcCommand = $"tellraw @a {json}";

                var sayRes = await _serverManagerService!.SendAsync("ABC", mcCommand).ReplaceMcTextToKook();
                await message.ExecuteKookSuccess(sayRes);
                break;
            default:


                return "找不到匹配指令";
        }

        return string.Empty;
    }

    private async Task<PlayerInfoEntity> GetPlayerInfoByKookId(string kookId)
    {
        return await SqlSugarHelper.Instance.Queryable<PlayerInfoEntity>()
                      .FirstAsync(a => a.KookUserId == kookId);
    }

    private async Task<bool> AnyBindKookToMc(string kookId)
    {
        return await SqlSugarHelper.Instance.Queryable<PlayerInfoEntity>()
                      .AnyAsync(a => a.KookUserId == kookId);
    }

    /// <summary>
    /// 发送验证码到服务器
    /// </summary>
    /// <param name="vcode"></param>
    /// <param name="mcName"></param>
    /// <returns></returns>
    private async Task<ICard> SendVcodeMsgToMc(int vcode, string mcName)
    {
        MessageComponent titleCcomponent = MessageComponent.Create()
                 .WithText($"[KOOK] ")
                 .WithColor(MinecraftColor.Green)
                 .WithFont("minecraft:default")
                 .WithClickEvent(ClickAction.OpenUrl, "https://www.kookapp.cn/app/invite/CQ0ojJ")
                 .WithHoverEvent(HoverAction.ShowText, $"[Kook频道] [点击加入]");


        MessageComponent senderCcomponent = MessageComponent.Create()
                .WithText($"[{DateTime.Now.ToString("HH:mm")}] 你正在进行kook绑定操作,你的验证码是：")
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
        var sayRes = await _serverManagerService!.SendAsync("ABC", mcCommand).ReplaceMcTextToKook();
        return sayRes;
    }

    /// <summary>
    /// mc玩家在线状态检测
    /// </summary>
    /// <param name="mcName"></param>
    /// <returns></returns>
    private async Task<bool> CheckOnline(string mcName)
    {
        var serverOnline = await _serverManagerService!.SendAsync("ABC", "list");
        if (!serverOnline.Contains(mcName)) return false;

        return true;
    }

    /// <summary>
    /// mc验证码校验
    /// </summary>
    /// <param name="reviceCode"></param>
    /// <param name="guildUserId"></param>
    /// <returns></returns>
    private async Task<string> CheckVcode(string reviceCode, string guildUserId)
    {

        var reviceCodeKey = $"bind:vode:{reviceCode}:{guildUserId}";
        var hasCode = await RedisHelper.ExistsAsync(reviceCodeKey);
        if (!hasCode) return "该验证码无效或已过期";

        var redisMcName = await RedisHelper.GetAsync(reviceCodeKey);

        //数据库用户判断
        var dbuInfo = await SqlSugarHelper.Instance.Queryable<PlayerInfoEntity>()
            .FirstAsync(a => a.Name == redisMcName);
        if (dbuInfo == null)
        {
            //新增
            dbuInfo = new PlayerInfoEntity()
            {
                Name = redisMcName,
                KookUserId = guildUserId
            };
            await SqlSugarHelper.Instance.Insertable(dbuInfo).ExecuteCommandAsync();
        }
        else
        {
            dbuInfo.KookUserId = guildUserId;
            dbuInfo.Name = redisMcName;
            await SqlSugarHelper.Instance.Updateable(dbuInfo).ExecuteCommandAsync();
        }



        return string.Empty;
    }

    /// <summary>
    /// 消息更新事件
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <param name="arg3"></param>
    /// <returns></returns>
    private async Task MessageUpdated(Cacheable<SocketMessage, Guid> arg1, Cacheable<SocketMessage, Guid> arg2, SocketTextChannel arg3)
    {
        ConsoleHelper.LogMessage("Kook消息更新事件", new string[] {
                $"服务器ID：{arg1.Id}",
                $"发送者： {arg1.Value.Author.Username}",
                $"消息：{arg1.Value.Content}  ---> {arg2.Value.Content}",
                $"原始内容: {arg1.Value.RawContent}",
                $"频道名称: {arg2.Value.Channel.Name}",
                $"时间戳: {arg2.Value.Timestamp}",
                $"修改时间戳: {arg2.Value.EditedTimestamp}"
            });
        await arg1.Value.AddReactionAsync(new Kook.Emoji("👀"));
    }
}

