using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineCosmos.Bot.Entity.Base;
using SqlSugar;

namespace MineCosmos.Bot.Entity
{
    /// <summary>
    /// 签到记录
    /// </summary>
    [Tenant("MinecosmosBot")]
    [SugarTable(TableName = "PlayerSingInRecord")]
    public class PlayerSingInRecordEntity : BaseEntity
    {
        /// <summary>
        /// 玩家ID
        /// </summary>
        public int PlayerId { get; set; }

        /// <summary>
        /// 积分
        /// </summary>
        public int Integral { get; set; }

        /// <summary>
        /// 幸运值
        /// </summary>
        public int LuckVal { get; set; }

        /// <summary>
        /// 幸运数
        /// </summary>
        public int LuckNumber { get; set; }

        /// <summary>
        /// 幸运颜色
        /// </summary>
        public string LuckColor { get; set; }

        /// <summary>
        /// 赠送绿宝石数量
        /// </summary>
        public int EmeraldVal { get; set; }
    }
}
