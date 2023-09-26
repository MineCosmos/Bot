using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineCosmos.Bot.Entity;

namespace MineCosmos.Bot.Service.Bot
{

    /// <summary>
    /// Bot服务，未来可能支持discord或者其他平台
    /// </summary>
    public interface IBotService: IBaseService
    {
        Task StartBot(string msg = "");
    }
}
