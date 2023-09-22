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

public interface IPlayerService : IBaseService
{
    /// <summary>
    ///  保存玩家信息
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<bool> SavePlayer(PlayerInfoEntity model);

    /// <summary>
    /// 获取玩家分页列表
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<ResPageDto<PlayerInfoEntity>> PagePlayer(ReqPageDto model);
}



