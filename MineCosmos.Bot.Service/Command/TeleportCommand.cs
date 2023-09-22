using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Kook;
using Kook.WebSocket;
using MineCosmos.Bot.Common;
using MineCosmos.Bot.Service.Bot;
using Newtonsoft.Json;

namespace MineCosmos.Bot.Service.Command;

/// <summary>
/// 传送功能
/// </summary>
public class TeleportCommand : BaseCommand, ICommand
{



    private string p1Name = string.Empty;
    private string p2Name = string.Empty;

    public TeleportCommand(CommandData data)
    {
        _data = data;
        string[] args = GetCommandArry();
    }

    public async Task<CommandExcuteResultModel> Execute()
    {
        var at = _data.SocketMessage.Tags.FirstOrDefault(a => a is { Type: TagType.UserMention });
        if (at is not null) return await TpTeleportAt(at);

        //p1Name => p2Name
        var res = await _data.ServerManager!.SendAsync("ABC", $"tpa {p1Name} {p2Name}").ReplaceMcTextToKook();
        return Success();
    }

    private async Task<CommandExcuteResultModel> TpTeleportAt([NotNull] ITag at)
    {
        string kookId = at.Key.ObjToString();

        var playerInfo = await GetPlayerInfoByKookIdAsync(kookId);

        var res = await _data.ServerManager!.SendAsync(_data.CurrentMcServer.Id, $"tpa {playerInfo.Name} {p2Name}").ReplaceMcTextToKook();

        return Success();

    }

}

