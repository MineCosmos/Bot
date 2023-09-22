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
using Serilog;
using Serilog.Events;

namespace MineCosmos.Bot.Service;
public class ServiceCentern : BaseService, IServiceCentern
{

    public ICommonService? _commonService;
    public IServerManagerService? _serverManagerService;
    public static KookSocketClient _client;
    public ServiceCentern(IServerManagerService serverManagerService, ICommonService commonService)
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

                Console.WriteLine(sb.ToString());

                //ABC服务器 （测试服务器）
                _ = _client.GetGuild(6079132046041885).GetTextChannel(4154290969579404)
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
            _client.MessageUpdated += MessageUpdated;
            _client.DirectMessageReceived += DirectMessageReceived;
            _client.MessageReceived += MessageReceived;
            _client.MessageButtonClicked += async (value, user, message, channel) =>
            {



                switch (value)
                {
                    case "changeAccount":

                        //     channel.SendTextAsync()

                        //     CardBuilder builder = new CardBuilder()
                        //.AddModule<SectionModuleBuilder>(s =>
                        //    s.WithText($"pong!"))
                        //.AddModule<ActionGroupModuleBuilder>(a => a
                        //    .AddElement(b => b
                        //        .WithClick(ButtonClickEventType.ReturnValue)
                        //        .WithText("点我！")
                        //        .WithValue("unique-id")
                        //        .WithTheme(ButtonTheme.Primary)));
                        break;

                    default: return;
                }
            };
            _client.Log += LogAsync;
            await _client.StartAsync();





        }
        catch (Exception ex)
        {
            ex.Message.PrintError();
            await _client.StopAsync();
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

    private async Task DirectMessageReceived(SocketMessage message, SocketUser socketUser, SocketDMChannel socketDMChannel)
    {
        if (socketUser.IsBot!.Value) await Task.CompletedTask;


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
        Console.WriteLine($"[{guildUser.Guild.Id}]{textChannel.Name} {guildUser.Nickname}/{guildUser.DisplayName} {message.Content}");
        #endregion

        //只处理文本信息
        if (message.Type != MessageType.Text && message.Type != MessageType.KMarkdown) return;

        //根据kook服务器和频道查询对应的MC服务器信息
        string guidId = guildUser.Guild.Id.ToString();
        string chanelId = textChannel.Id.ToString();


        #region 指令前置处理

        //转换一下指令
        string command = message.Content.ToLower()
            .Replace("帮助", "help")
            .Replace("传送", "tp")
            .Replace("绑定", "BindAccount")
            .Replace("bind", "BindAccount")
            .Replace("说话", "say")
            .Replace("皮肤", "skin")
            .Replace("自杀", "killself");

        //匹配特殊的纯数字验证码
        Regex regex = new Regex(@"^\d{5}$");
        if (regex.IsMatch(command))
            command = $"CheckCode {command}";
        #endregion

        string[] parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        #region 频道初始化指令 TODO 方法封装
        // 示例："服务器注册 xx.xx.cn:999 cc.cc.cc.cn:888 123456"
        if (parts[0] == "服务器注册教程")
        {
            await message.Channel.SendTextAsync($"服务器注册 服务器地址:端口 RCON地址:端口 RCON密码");
            return;
        }
        if (parts[0] == "服务器注册")
        {

            try
            {
                if (parts.Length < 4) { throw new ArgumentException("服务器注册指令长度不足"); }
                string mcServerIp = parts[1].Split(':')[0].ToString()
                    ?? throw new ArgumentException("MC服务器IP不可为空");
                string mcServerPort = parts[1].Split(':')[1].ToString()
                    ?? throw new ArgumentException("MC服务器端口不可为空");
                string mcServerRconIp = parts[2].Split(':')[0].ToString()
                     ?? throw new ArgumentException("Rcon的IP不可为空");
                string mcServerRconPort = parts[2].Split(':')[1].ToString()
                    ?? throw new ArgumentException("Rcon的端口不可为空");
                string mcServerRconPwd = parts[3].ToString()
                    ?? throw new ArgumentException("Rcon的密码不可为空");
                string mcServerName = parts[4].ToString() ?? $"{mcServerIp}服务器_${DateTime.Now.ToString("yyyyMMddHHmmss")}";

                var appendMcServer = await _serverManagerService!.AppendServer(new()
                {
                    ServerName = mcServerName,
                    KookGuild = guidId,
                    KookChannelId = chanelId,
                    ServerIp = mcServerIp,
                    ServerPort = mcServerPort,
                    RconAddress = mcServerRconIp,
                    RconPort = mcServerRconPort,
                    RconPwd = mcServerRconPwd
                });
                if (appendMcServer is null) throw new ArgumentException("注册失败辣！应该是重名🤨");

            }
            catch (ArgumentException ex)
            {
                await message.AddReactionAsync(new Kook.Emoji("🤣"));
                await message.Channel.SendTextAsync(ex.ToString());
            }
            catch (Exception ex)
            {
                await message.AddReactionAsync(new Kook.Emoji("😣"));
                await message.Channel.SendTextAsync($"程序非预估异常：{ex.Message}");
            }
            finally
            {
                await message.DeleteAsync();
                await message.Channel.SendTextAsync("注册完成，如未成功请检查失败原因后重试 √");
            }
            return;
        }

        #endregion

        MinecraftServerEntity? mcServerInfo = await SqlSugarHelper.Instance
              .Queryable<MinecraftServerEntity>()
              .FirstAsync(a => a.KookGuild == guidId && a.KookChannelId == chanelId);
        if (mcServerInfo is null) return;

        #region 限定频道(用于测试
        //string codeChannel = AppSettings.app(new string[] { "Kook", "CodeChannel" });
        //if (textChannel.Name != codeChannel) return;
        #endregion


        var result = await CommandInterpreter.Interpret(new CommandData
        {
            CommandName = parts[0],
            SocketGuildUser = guildUser,
            SocketMessage = message,
            ServerManager = _serverManagerService,
            CurrentMcServer = mcServerInfo
        });

        await message.AddReactionAsync(new Kook.Emoji("⌛"));
        if (result.Success && !result.IsReplyMsg)
        {
            await message.AddReactionAsync(new Kook.Emoji("😍"));
        }
        else if (!result.Success)
        {
            await message.AddReactionAsync(new Kook.Emoji("😨"));
            if (result.Message.IsNotEmptyOrNull()) await textChannel.SendTextAsync(result.Message);
        }
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

