using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineCosmos.Bot.Entity.Base;
using SqlSugar;

namespace MineCosmos.Bot.Entity
{

    /// <summary>
    /// 指令组
    /// </summary>
    [SugarTable(TableName = "CommandGroup")]
    public class CommandGroupEntity : BaseEntity
    {
        /// <summary>
        /// 服务器ID
        /// </summary>
        public int ServerId { get; set; }

        /// <summary>
        /// 指令名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 显示排序
        /// </summary>
        public int Sort { get; set; }


        /// <summary>
        /// 导航属性，禁止手动赋值
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(CommandGroupItemEntity.GroupId))]
        public List<CommandGroupItemEntity> GroupItems { get; set; }
    }
}
