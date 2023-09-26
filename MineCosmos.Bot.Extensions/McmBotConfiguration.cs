using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgileConfig.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MineCosmos.Bot.Common;
using MineCosmos.Bot.Entity;
using MineCosmos.Bot.Service;
using MineCosmos.Bot.Service.Bot;
using MineCosmos.Bot.Service.Common;
using Serilog;
using SqlSugar;

namespace MineCosmos.Bot.Extensions
{
    public static class McmBotConfiguration
    {
        

        public static IHostBuilder UseBotConfigureServices(this IHostBuilder hostBuilder)
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.File("log.txt",
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true)
                    .CreateLogger();
            string? environmentVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Log.Debug($"当前环境:{environmentVariable}");

            hostBuilder
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();
                    if (environmentVariable.IsNullOrEmpty())
                        configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
                    else
                        configuration.AddJsonFile($"appsettings.{environmentVariable}.json", optional: true, reloadOnChange: false);
                })
                .ConfigureServices((hostingContext, services) =>
              {
                  services.TryAddSingleton(new AppSettings(hostingContext.Configuration));
                  services.TryAddSingleton<ICommonService, CommonService>();
                  services.TryAddSingleton<IServerManagerService, ServerManagerService>();
                  services.TryAddSingleton<ICommandManagerService, CommandManagerService>();
                  services.TryAddSingleton<IPlayerService, PlayerService>();
              });

            if (AppSettings.app(new string[] { "AgileConfig", "Enable" }).ObjToBool())
            {
                Console.WriteLine($"启用AgileConfig配置中心");
                hostBuilder.UseAgileConfig(new ConfigClient($"appsettings.agileconfig.json"));
            }

            #region    初始化表 (TODO:做成开关
            SqlSugarHelper.Instance.CodeFirst.InitTables(
            typeof(PlayerInfoEntity),
            typeof(CommandGroupEntity),
            typeof(CommandGroupItemEntity),
            typeof(PlayerSingInRecordEntity),
            typeof(TaskQueueEntity),
            typeof(MinecraftServerEntity),
            typeof(TimeEventHandleEntity)
            );
            #endregion

            return hostBuilder;
        }

        public static void AddBotService(this IServiceCollection hostBuilder)
        {
            hostBuilder.TryAddSingleton<IBotService, KookBotService>();
        }

    }
}
