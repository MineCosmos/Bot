using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineCosmos.Bot.Entity.Base;
using SqlSugar;

namespace MineCosmos.Bot.Entity;

[Tenant("MinecosmosBot")]
[SugarTable(TableName = "PlayerInfo")]
public class PlayerInfoEntity : BaseEntity
{
    /// <summary>
    /// 名字
    /// </summary>
    [DisplayName("玩家名称")]
    public string Name { get; set; }

    [SugarColumn(IsNullable = true), DisplayName("正版ID")]
    public string Uuid { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    [SugarColumn(IsNullable = true), DisplayName("头像")]
    public string? Avatar { get; set; }

    /// <summary>
    /// 签到次数
    /// </summary>
    [DisplayName("签到次数")]
    public int SignInCount { get; set; }

    /// <summary>
    /// 绿宝石数量
    /// </summary>
    [SugarColumn(IsNullable = true), DisplayName("绿宝石数量")]
    public int EmeraldVal { get; set; }

    /// <summary>
    /// kook id
    /// </summary>
    [DisplayName("KOOK用户ID")]
    public string? KookUserId { get; set; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    [DisplayName("所在KOOK服务器ID")]
    public int ServerId { get; set; }

    /// <summary>
    /// 服务器信息
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(ServerId))]
    public MinecraftServerEntity ServerInfo { get; set; }

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



