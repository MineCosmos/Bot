using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineCosmos.Bot.Entity.Base;
using SqlSugar;

namespace MineCosmos.Bot.Entity;

[Tenant("MinecosmosBot")]
[SugarTable(TableName ="PlayerInfo")]
public class PlayerInfoEntity : BaseEntity
{
    /// <summary>
    /// 名字
    /// </summary>
    public string Name { get; set; }

    [SugarColumn(IsNullable = true)]
    public string Uuid { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    [SugarColumn(IsNullable = true)]
    public string? Avatar { get; set; }

    /// <summary>
    /// 签到次数
    /// </summary>
    public int SignInCount { get; set; }

    /// <summary>
    /// 绿宝石数量
    /// </summary>
    public int? EmeraldVal { get; set; }

    /// <summary>
    /// kook id
    /// </summary>
    public string? KookUserId { get; set; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    public int ServerId { get; set; }

    /// <summary>
    /// 平台列表
    /// </summary>
    [Navigate(NavigateType.OneToMany, nameof(PlayerPlatformEntity.PlayerId))]
    public List<PlayerPlatformEntity> ListPlayerPlatformEntity { get; set; }
}


[Tenant("MinecosmosBot")]
[SugarTable(TableName = "PlayerPlatform")]
public class PlayerPlatformEntity : BaseEntity
{
    /// <summary>
    /// 名字
    /// </summary>
    public string PlatformName { get; set; }

    /// <summary>
    /// 平台ID（QQ号之类的
    /// </summary>
    public string PlatformId { get; set; }

    /// <summary>
    /// 玩家ID
    /// </summary>
    public int PlayerId { get; set; }
}



