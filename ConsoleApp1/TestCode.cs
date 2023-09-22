using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MineCosmos.Bot
{
    internal class TestCode
    {
    }




    // 定义一个委托，表示当接收到一个协议对象时执行的操作
    public delegate void CommandReceivedHandler(Command command);

    // 定义一个协议对象，用于封装解析后的命令信息
    public class Command
    {
        public string Name { get; set; }
        public List<string> Args { get; set; }
        public Dictionary<string, string> Options { get; set; }

        public Command()
        {
            Args = new List<string>();
            Options = new Dictionary<string, string>();
        }
    }

    // 定义一个工具类，用于解析协议字符串
    public static class CommandParser
    {
        public static Command Parse(string protocol)
        {
            var command = new Command();
            var parts = protocol.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // 解析命令名
            command.Name = parts[0];

            // 解析选项和选项参数
            for (int i = 1; i < parts.Length; i++)
            {
                if (parts[i].StartsWith("-"))
                {
                    var optionName = parts[i].Substring(1);
                    var optionValue = "";

                    if (i + 1 < parts.Length && !parts[i + 1].StartsWith("-"))
                    {
                        optionValue = parts[i + 1];
                        i++;
                    }

                    command.Options[optionName] = optionValue;
                }
                else
                {
                    // 解析参数
                    command.Args.Add(parts[i]);
                }
            }

            return command;
        }
    }

    // 定义一个事件总线，用于发布协议对象
    public static class EventBus
    {
        public static event CommandReceivedHandler? CommandReceived;

        public static void Publish(Command command)
        {
            CommandReceived?.Invoke(command);
        }
    }

    // 定义一个Handler类，用于处理协议对象
    public static class CommandHandler
    {
        [CommandHandler("help")]
        public static void HandleHelpCommand(Command command)
        {
            Console.WriteLine("This is a help message.");
        }

        [CommandHandler("test")]
        public static string HandleTestCommand(Command command)
        {
            Console.WriteLine($"This is Test,mine arg : {command.Args[0]} {command.Args[1]}");
        
            return "";
        }
    }

    /// <summary>
    /// 用于标记Handler方法的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CommandHandlerAttribute : Attribute
    {
        public string CommandName { get; }

        public CommandHandlerAttribute(string commandName)
        {
            CommandName = commandName;
        }
    }

    /// <summary>
    /// 命令解析器，用于解析协议字符串并调用相应的Handler方法
    /// </summary>
    public static class CommandInterpreter
    {
        /// <summary>
        /// 解析协议字符串并调用相应的Handler方法
        /// </summary>
        /// <param name="protocol"></param>
        public static void Interpret(string protocol)
        {
            var command = CommandParser.Parse(protocol);

           


            // 查找匹配的Handler方法，并调用
            var handlers = typeof(CommandHandler).GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(CommandHandlerAttribute), false)
                    .Cast<CommandHandlerAttribute>()
                    .Any(a => a.CommandName == command.Name));

            foreach (System.Reflection.MethodInfo handler in handlers)
            {
                handler.Invoke(null, new object[] { command });
            }
        }
    }


    /// <summary>
    /// 测试类，用于测试命令解析器
    /// </summary>
    public static class Test
    {
        public static void Main()
        {
            // 基于此消息创建命令上下文
            //var context = new SocketCommandContext(_client, message);


            // 订阅CommandReceived事件
            EventBus.CommandReceived += (command) =>
            {
                CommandInterpreter.Interpret(command.Name + " " + string.Join(" ", command.Args) +
                    " " + string.Join(" ", command.Options.Select(kv => "-" + kv.Key + " " + kv.Value)));
            };

            // 发布协议对象
            var command1 = new Command { Name = "help" };
            EventBus.Publish(command1);

            var command2 = new Command { Name = "test", Args = new List<string> { "arg1", "arg2" } };
            EventBus.Publish(command2);

            var command3 = new Command
            {
                Name = "test",
                Options = new Dictionary<string, string> { { "option1", "value1" }, { "option2", "value2" } }
            };
            EventBus.Publish(command3);
        }
    }
}
