using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kook.WebSocket;

namespace MineCosmos.Bot.Service;

public interface ICommand
{
    Task<CommandExcuteResultModel> Execute();
}

