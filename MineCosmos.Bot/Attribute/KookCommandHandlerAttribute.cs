using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineCosmos.Bot;


/// <summary>
/// 用于标记Handler方法的特性
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class KookCommandHandlerAttribute : Attribute
{
    public string CommandName { get; }

    public KookCommandHandlerAttribute(string commandName)
    {
        CommandName = commandName;
    }
}

