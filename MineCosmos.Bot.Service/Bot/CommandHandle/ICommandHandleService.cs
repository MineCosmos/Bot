using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineCosmos.Bot.Service.Bot
{
    public interface ICommandHandleService: IBaseService
    {
        Task<Stream> GetMinecraftSkin(string uuid);
        Task<UUIDModel?> GetMinecraftUUID(string message);
    }
}
