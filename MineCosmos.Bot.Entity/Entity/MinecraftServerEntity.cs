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
    /// Minecraft服务器信息
    /// </summary>
    [Tenant("MinecosmosBot")]
    [SugarTable(TableName = "MinecraftInfoServer")]
    public class MinecraftServerEntity : BaseEntity
    {
        /// <summary>
        /// 服务器名
        /// </summary>
        [DisplayName("服务器名称"),Required(ErrorMessage ="服务器名不可为空")]
        public string ServerName { get; set; }
        /// <summary>
        /// 服务器IP
        /// </summary>
        [DisplayName("服务IP地址"), Required(ErrorMessage = "服务IP地址不可为空")]
        public string ServerIp { get; set; } = "25565";
        /// <summary>
        /// 服务器端口
        /// </summary>
        [DisplayName("服务器端口"),Required(ErrorMessage = "服务器端口不可为空")]
        public string ServerPort { get; set; }
        /// <summary>
        /// RCON地址
        /// </summary>
        [DisplayName("RCON地址"), Required(ErrorMessage = "RCON地址不可为空")]
        public string RconAddress { get; set; }

        /// <summary>
        /// RCON端口
        /// </summary>
        [DisplayName("RCON端口"), Required(ErrorMessage = "RCON端口不可为空")]
        public string RconPort { get; set; } = "25575";

        /// <summary>
        /// RCON密码
        /// </summary>
        [DisplayName("RCON密码"), Required(ErrorMessage = "RCON密码不可为空")]
        public string RconPwd { get; set; }

        /// <summary>
        /// Kook的服务器ID
        /// </summary>
        [DisplayName("Kook服务器"), Required(ErrorMessage = "Kook服务器不可为空")]
        public string KookGuild { get; set; }

        /// <summary>
        /// Kook的频道ID（为的是一个kook服务器多个mc服务器
        /// </summary>
        [DisplayName("Kook频道"), Required(ErrorMessage = "Kook频道不可为空")]
        public string KookChannelId { get; set; }

        /// <summary>
        /// 一些备注说明
        /// </summary>       
        [SugarColumn(ColumnDataType = "text",IsNullable = true),DisplayName("说明")]
        public string Remark { get; set; }
    }
}
