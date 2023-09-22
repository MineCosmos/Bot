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

/// <summary>
/// 玩家服务
/// </summary>
public class PlayerService : BaseService, IPlayerService
{
    /// <summary>
    ///  保存玩家信息
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<bool> SavePlayer(PlayerInfoEntity model)
    {
        bool hasVal = await AnyAsync<PlayerInfoEntity>(
            a => a.Uuid == model.Uuid && (model.Id < 0 || a.Id != model.Id));

        //TODO 封装异常
        if (hasVal) throw new Exception("已存在相同uuid玩家");

        return await Save<PlayerInfoEntity>(model);
    }

    /// <summary>
    /// 获取玩家分页列表
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<ResPageDto<PlayerInfoEntity>> PagePlayer(ReqPageDto model) => await GetPageDataAsync<PlayerInfoEntity>(model, a => model.Key == "" || a.Name.Contains(model.Key));
}



