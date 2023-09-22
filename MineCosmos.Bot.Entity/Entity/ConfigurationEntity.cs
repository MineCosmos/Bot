using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace MineCosmos.Bot.Entity
{
    /// <summary>
    /// 配置表
    /// </summary>
    [Tenant("MinecosmosBot")]
    [SugarTable(TableName = "Configuration")]
    internal class ConfigurationEntity
    {
        /// <summary>
        /// 父级ID
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Val { get; set; }
        /// <summary>
        /// 配置描述
        /// </summary>
        public string Description { get; set; }


    }
}
