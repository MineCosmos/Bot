using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineCosmos.Bot.Entity.Base;
using SqlSugar;

namespace MineCosmos.Bot.Entity;
[TenantAttribute("MinecosmosBot")]
[SugarTable(TableName = "Cache")]
public class CacheEntity:BaseEntity
{
    /// <summary>
    /// 键
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public string Val { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    public DateTime Expiration { get; set; }
}

