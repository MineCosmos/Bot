using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Kook;
using Kook.WebSocket;
using Mapster;
using MineCosmos.Bot.Common;
using MineCosmos.Bot.Service.Bot;
using Newtonsoft.Json;

namespace MineCosmos.Bot.Service.Command;
public class InfoCommand : BaseCommand, ICommand
{

    private readonly IServerManagerService? _serverManagerService;

    public InfoCommand(CommandData data)
    {
        _data = data;
    }

    public async Task<CommandExcuteResultModel> Execute()
    {
        List<IModuleBuilder> moduleBuilders = new List<IModuleBuilder>();

        moduleBuilders.Add(new SectionModuleBuilder
        {
            Text = new KMarkdownElementBuilder { 
                Content = $"🔶 服务器名称：{_data.CurrentMcServer.ServerName}"
            }
        });

        moduleBuilders.Add(new SectionModuleBuilder
        {
            Text = new KMarkdownElementBuilder
            {
                Content = $"🔶 服务器地址：{_data.CurrentMcServer.ServerIp}:{_data.CurrentMcServer.ServerPort}"
            }
        });

        moduleBuilders.Add(new SectionModuleBuilder
        {
            Text = new KMarkdownElementBuilder
            {
                Content = $"{_data.CurrentMcServer.Remark}"
            }
        });
        CardBuilder builder = new CardBuilder()
        { Modules = moduleBuilders, Theme = CardTheme.Secondary, Size = CardSize.Large };

        await _data.SocketMessage!.Channel.SendCardAsync(builder.Build());

        return Success();
    }

}

