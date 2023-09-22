using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kook.WebSocket;
using MineCosmos.Bot.Entity;
using MineCosmos.Bot.Service.Command;
using MineCosmos.Bot.Service.Bot;
using MineCosmos.Bot.Service;

namespace MineCosmos.Bot;


public class CommandData
{
    public IServerManagerService ServerManager { get; set; }
    public SocketGuildUser SocketGuildUser { get; set; }
    public SocketMessage SocketMessage { get; set; }
    public string CommandName { get; set; }

    /// <summary>
    /// 当前对应的mc服务器
    /// </summary>
    public MinecraftServerEntity CurrentMcServer { get; set; }
}

/// <summary>
/// CommandInterpreter
/// </summary>
public static class CommandInterpreter
{
    private static readonly Dictionary<string, Type> CommandTypes = new Dictionary<string, Type>();

    static CommandInterpreter()
    {
        // 注册命令类型和对应的指令关键字
        RegisterCommand("help", typeof(HelpCommand));
        RegisterCommand("say", typeof(SayCommand));
        RegisterCommand("bindaccount", typeof(BindAccountCommand));
        RegisterCommand("checkcode", typeof(CheckCodeCommand));
        RegisterCommand("tp", typeof(TeleportCommand));
    }

    // public static void RegisterCommand(CommandTypesModel model) => CommandTypes.Add(model);
    public static void RegisterCommand(string commandName, Type commandType)
    {
        CommandTypes[commandName] = commandType;
    }

    public static async Task<CommandExcuteResultModel> Interpret(CommandData model)
    {
        if (CommandTypes.TryGetValue(model.CommandName.ToLower(), out Type? commandType))
        {
            if (commandType is null) return new CommandExcuteResultModel { IsReplyMsg = true, Message = "找不到匹配的指令类型", Success = false }; ;

            if (Activator.CreateInstance(commandType, model) is ICommand command)
            {
                return await command.Execute();
            }
            else
            {
                return new CommandExcuteResultModel { IsReplyMsg = true, Message = "指令匹配失败", Success = false };
            }
        }
        else
        {
            return new CommandExcuteResultModel { IsReplyMsg = true, Message = "无效的指令", Success = false };
        }
    }
}
