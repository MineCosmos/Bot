using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace MineCosmos.Bot.Service
{
    public class SqlSugarConfig : ConnectionConfig
    {
        /// <summary>
        /// 是否初始化数据库
        /// </summary>
        public bool IsInitDb { get; set; }

        /// <summary>
        /// 是否初始化种子数据
        /// </summary>
        public bool IsSeedData { get; set; }

        /// <summary>
        /// 是否驼峰转下划线
        /// </summary>
        public bool IsUnderLine { get; set; }
    }
}
