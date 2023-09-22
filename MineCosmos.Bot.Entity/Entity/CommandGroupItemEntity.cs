using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineCosmos.Bot.Entity.Base;
using SqlSugar;

namespace MineCosmos.Bot.Entity;

/// <summary>
/// 指令组Item
/// </summary>   
[SugarTable(TableName = "CommandGroupItem")]
public class CommandGroupItemEntity : BaseEntity
{
    /// <summary>
    /// 指令组ID
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    ///  实际命令
    /// </summary>
    public int Command { get; set; }

    /// <summary>
    /// 执行顺序
    /// </summary>
    public int ExcuteSort { get; set; }

    /// <summary>
    /// 执行前等待秒数
    /// </summary>
    public int BeforeWaitSecond { get; set; } = 0;
}

