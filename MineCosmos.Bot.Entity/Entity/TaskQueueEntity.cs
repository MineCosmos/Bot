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
    /// 任务队列
    /// </summary>
    [Tenant("MinecosmosBot")]
    [SugarTable(TableName = "TaskQueue")]
    public class TaskQueueEntity:BaseEntity
    {
        /// <summary>
        /// 类型 0：文字
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 接收到的消息
        /// </summary>
        public string ReviceMsg { get; set; }

        /// <summary>
        /// 0 群，1好友
        /// </summary>
        public int GroupOrPrivate { get; set; }

        /// <summary>
        /// 发送者QQ
        /// </summary>
        public string SenderId { get; set; }
        public string SenderGroupId { get; set; }

        public string ReplyId { get; set; }
        public string ReplyGroupId { get; set; }    

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime ExcuteTime { get; set; }

        /// <summary>
        /// 是否已经执行
        /// </summary>
        public int IsExcute { get; set; }
    }
}
