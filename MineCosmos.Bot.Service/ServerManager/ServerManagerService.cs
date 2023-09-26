using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CoreRCON;
using Mapster.Models;
using MineCosmos.Bot.Entity;
using MineCosmos.Bot.Entity.Dto;
using SqlSugar;

namespace MineCosmos.Bot.Service;

public class ServerManagerService : BaseService, IServerManagerService
{
    /// <summary>
    /// 添加服务器
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<MinecraftServerEntity> AppendServer(MinecraftServerEntity model)
    {
        if (!await _db.Queryable<MinecraftServerEntity>().AnyAsync(a => a.ServerName == model.ServerName))
        {
            return await _db.Insertable(model).ExecuteReturnEntityAsync();
        }

        return null;
    }

    /// <summary>
    /// 保存服务器信息
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<bool> SaveServer(MinecraftServerEntity model)
    {
        var res = await _db.Storageable(model).ExecuteCommandAsync();
        return res > 0;
    }

    /// <summary>
    /// 获取服务器分页列表
    /// </summary>
    /// <param name="pageNum"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public async Task<ResPageDto<MinecraftServerEntity>> PageServers(ReqPageDto model) => await GetPageDataAsync<MinecraftServerEntity>(model, a => a.Id > 0);

    /// <summary>
    /// 获取服务器列表
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<List<MinecraftServerEntity>> GetListServer(string name ="")
    {
      var lst = await  GetListAsync<MinecraftServerEntity>(a => name == "" || a.ServerName.Contains(name));
        return lst;
    }

    #region 服务器执行RCON指令

    /// <summary>
    /// 通过RCON执行指令，返回执行结果
    /// </summary>
    /// <param name="serverName"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<string> SendAsync(int serverId, string command)
    {
        var serverInfo = await _db.Queryable<MinecraftServerEntity>().FirstAsync(a => a.Id == serverId);
        if (serverInfo == null)
        {
            return "找不到服务器";
        }
        ushort.TryParse(serverInfo.RconPort, out ushort port);
        var rconClient = new RCON(IPAddress.Parse(serverInfo.RconAddress), port, serverInfo.RconPwd);
        await rconClient.ConnectAsync();
        var response = await rconClient.SendCommandAsync(command);
        return response;
    }


    /// <summary>
    /// 通过RCON执行指令，返回执行结果
    /// </summary>
    /// <param name="serverName"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task<string> SendAsync(string serverName, string command)
    {
        var serverInfo = await _db.Queryable<MinecraftServerEntity>().FirstAsync(a => a.ServerName == serverName);
        if (serverInfo == null)
        {
            return "找不到服务器";
        }
        ushort.TryParse(serverInfo.RconPort, out ushort port);
        var rconClient = new RCON(IPAddress.Parse(serverInfo.RconAddress), port, serverInfo.RconPwd);
        await rconClient.ConnectAsync();
        var response = await rconClient.SendCommandAsync(command);
        return response;
    }

    #endregion
}



