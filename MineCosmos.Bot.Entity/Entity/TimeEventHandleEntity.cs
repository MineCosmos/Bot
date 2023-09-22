using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace MineCosmos.Bot.Entity
{
    /// <summary>
    /// 时间事件处理
    /// </summary>
    [Tenant("MinecosmosBot")]
    [SugarTable(TableName = "TimeHandle")]
    public class TimeEventHandleEntity
    {
        /// <summary>
        /// 发生时间
        /// </summary>
        public string OccurrenceTime { get; set; }

        /// <summary>
        /// 处理类型 0：时分  1：日月
        /// </summary>
        public int HandleType { get; set; }
    }
}
