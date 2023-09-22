using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineCosmos.Bot.Common;
using MineCosmos.Bot.Entity;
using MineCosmos.Bot.Service.Bot;

namespace MineCosmos.Bot.Service;

/// <summary>
/// 账号绑定操作
/// </summary>
public class BindAccountCommand : BaseCommand, ICommand
{
    string[] args;
    string playerName;

    public BindAccountCommand(CommandData data)
    {
        _data = data;

        args = GetCommandArry();
        if (args[0].IsNullOrEmpty())
        {
            //TODO 自定义异常类型
            throw new Exception("参数不正确");
        }

        playerName = args[0];
    }

    public async Task<CommandExcuteResultModel> Execute()
    {
        PlayerInfoEntity? dbUserInfo = await Db.Queryable<PlayerInfoEntity>()
               .FirstAsync(expression: a => a.Name == playerName);

        //如果没装插件，则默认假设玩家在服务器
        var checkOnline = await _data.ServerManager!.SendAsync(
            _data.CurrentMcServer.ServerName,
            $"kook checkonline {playerName}");

        if (checkOnline == "0" || checkOnline.IsNullOrEmpty())
            return Error("此功能需要玩家在服务器，你并没有在线，请登录游戏后重试");

        //给游戏服务器内玩家发送验证码
        await SendCodeToMinecraftServer(playerName);

        if (dbUserInfo != null && dbUserInfo!.KookUserId!.IsNotEmptyOrNull())
        {
            await _data.SocketMessage.ExecuteKookSuccess($"你已经绑定过{dbUserInfo.Name},此举将会更换绑定，验证码已经发送，请在游戏内查收,");
        }
        else
        {
            await _data.SocketMessage.ExecuteKookSuccess("验证码发送成功，请在游戏内查收");
        }

        return Success();
    }


}

