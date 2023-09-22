using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineCosmos.Bot.Entity;
using MineCosmos.Bot.Entity.Dto;

namespace MineCosmos.Bot.Service;

public interface IServerManagerService: IBaseService
{
    /// <summary>
    /// 获取服务器分页列表
    /// </summary>
    /// <param name="pageNum"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    Task<ResPageDto<MinecraftServerEntity>> PageServers(ReqPageDto model);
    Task<MinecraftServerEntity> AppendServer(MinecraftServerEntity model);
    Task<string> SendAsync(string serverName, string command);
    Task<string> SendAsync(int serverId, string command);
    /// <summary>
    /// 保存服务器信息
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<bool> SaveServer(MinecraftServerEntity model);

}
