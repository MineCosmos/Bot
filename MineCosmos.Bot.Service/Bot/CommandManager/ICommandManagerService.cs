using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineCosmos.Bot.Entity;

namespace MineCosmos.Bot.Service.Bot
{
    public interface ICommandManagerService : IBaseService
    {
        Task<CommandGroupEntity> GetCommandGroupId(int id);
        Task<List<CommandGroupEntity>> GetListCommandGroupByServerId(int serverId);
        Task<bool> RemoveCommandGroup(int id);
        Task<(bool isSuccess, string errorMsg)> SaveCommandGroup(CommandGroupEntity model);
    }
}
