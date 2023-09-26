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
        [DisplayName("服务器ID")]
        public int ServerId { get; set; }

        /// <summary>
        /// 指令名称
        /// </summary>
        [DisplayName("指令组名称")]
        public string Name { get; set; }

        /// <summary>
        /// 显示排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否通用（所有服务器公用
        /// </summary>
        public bool IsCommon { get; set; }

        /// <summary>
        /// 继承的父级ID
        /// </summary>
        public int Pid { get; set; } = 0;

        /// <summary>
        /// Child
        /// https://www.donet5.com/Home/Doc?typeId=2311
        /// </summary>
        public CommandGroupEntity Child { get; set; }

        /// <summary>
        /// 所属服务器
        /// </summary>
        [SugarColumn(IsIgnore = true),Navigate(NavigateType.OneToOne, nameof(ServerId))]
        public MinecraftServerEntity ServerInfo { get; set; } 

        /// <summary>
        /// 指令明细
        /// </summary>
        [SugarColumn(IsIgnore = true), Navigate(NavigateType.OneToMany, nameof(CommandGroupItemEntity.GroupId))]
        public List<CommandGroupItemEntity> GroupItems { get; set; }
    }
}
