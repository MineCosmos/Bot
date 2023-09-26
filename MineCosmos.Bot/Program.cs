#nullable disable
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MineCosmos.Bot.Entity;
using MineCosmos.Bot.Service;
using Sora.Net.Config;
using Sora;
using Spectre.Console;
using Sora.Util;
using MineCosmos.Bot.Interactive;
using Sora.Entities;
using Sora.Entities.Segment;
using Newtonsoft.Json;
using SqlSugar;
using MineCosmos.Bot.Service.Bot;
using MineCosmos.Bot.Service.Common;
using MineCosmos.Bot.Helper;
using MineCosmos.Bot.Common;
using Serilog;
using AgileConfig.Client;
using AgileConfig.Client.RegisterCenter;
using Microsoft.AspNetCore.Hosting;
using MineCosmos.Bot;
using MineCosmos.Bot.Extensions;

Console.WriteLine("[MineCosmos Bot Center]");

IHost host = Host.CreateDefaultBuilder(args)
      .ConfigureServices((hostingContext, services) =>
      {
          //同理，blazor那边不启动kook
          if (AppSettings.app(new string[] { "Kook", "Enable" }).ObjToBool())
          {
              services.AddBotService();
          }

      })
    .UseBotConfigureServices()
    .UseSerilog(Log.Logger, dispose: true)
    .Build();

//agile的服务发现，blazor那边不用，未来这里写几个简单的miniapi
IRegisterService regService = host.Services.GetService<IRegisterService>();
await regService?.RegisterAsync();



if (AppSettings.app(new string[] { "Kook", "Enable" }).ObjToBool())
{
    await host.Services.GetService<IBotService>().StartBot();
}

//var server = await serverManagerService!.AppendServer(new MinecraftServerEntity()
//{
//    ServerName = "ABC",
//    ServerIp = "43.248.184.171",
//    ServerPort = "23335",
//    RconPwd = "Dujiord568wejdu907foiu873",
//    RconAddress = "43.248.184.171",
//    RconPort = "9027",
//});
//await serverManagerService.SendAsync("ABC", "say rcon控制已启动1");
//var res = await serverManagerService.SendAsync("ABC", "execute as GreatMingCtu run tp  GreatMingCtu");

#region 后期功能，模板解析
////模板实体
//var obj = new
//{
//    PlayerName1 = "GreatMingCtu",
//    PlayerName2 = "GreatMingNing"
//};
////指令模板设置
//var testSetting = new List<object>
//{
//    new {
//            templateParamName = "P1",
//            replace = "",
//            systemParamName = "PlayerName1"
//        },
//    new {
//            templateParamName = "P2",
//            replace = "",
//            systemParamName = "PlayerName2"
//        },
//};
////解析绑定最终执行
//var sysParam = JObject.FromObject(obj);
//Func<JToken,(string param,string val)> createItem = obj =>
//{
//    var key = obj.Value<string>("templateParamName");
//    var value = obj.Value<string>("replace");

//    //替换字符串为空，并且传入参数不为空
//    if (value!.IsNullOrEmpty() && sysParam != null)
//    {
//        var sysKey = obj.Value<string>("systemParamName");
//        if (sysKey!.IsNotEmptyOrNull())
//            value = Convert.ToString(sysParam.SelectToken(sysKey));
//    }
//    return (key, value);
//};
//var lst = JArray.Parse(JsonConvert.SerializeObject(testSetting));
//var dics = lst.Select(createItem).ToList();
//var testRconCommandTemplate_3 = $"/execute as @a run tp #P1# #P2#; say #P1# 小号传送辣！";
//foreach (var kvp in dics)
//{
//    string key = kvp.param;
//    string value = kvp.val;
//    testRconCommandTemplate_3 = testRconCommandTemplate_3.Replace($"#{key}#", value);
//}
#endregion

#region redis
if (AppSettings.app(new string[] { "RedisConfig", "Enable" }).ObjToBool())
{
    //开启redis
    var csredis = new CSRedis.CSRedisClient(AppSettings.app(new string[] { "RedisConfig", "ConnectionString" }));
    RedisHelper.Initialization(csredis);
}
#endregion

#region SelfSocket

_ = Task.Run(() =>
{
    //// 创建一个 IP 地址对象，表示要监听的主机和端口
    //IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
    //int port = 7415;
    //IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

    //// 创建一个 Socket 对象
    //Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

    //// 将 Socket 绑定到要监听的地址和端口
    //listener.Bind(localEndPoint);

    //// 开始监听传入的连接请求
    //listener.Listen(10);

    //Console.WriteLine("Waiting for a connection...");

    //while (true)
    //{
    //    // 接受传入的连接请求，并创建一个新的 Socket 对象来处理连接
    //    Socket handler = listener.Accept();

    //    // 处理连接的逻辑
    //    Console.WriteLine($"Client connected from {handler.RemoteEndPoint}");

    //    byte[] buffer = new byte[1024];
    //    int bytesReceived = handler.Receive(buffer);
    //    string data = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
    //    Console.WriteLine($"Received data: {data}");

    //    // 发送回复消息到客户端
    //    string replyMessage = "Server received your message: " + data;
    //    byte[] replyBuffer = Encoding.ASCII.GetBytes(replyMessage);
    //    handler.Send(replyBuffer);

    //    // 关闭连接
    //    handler.Shutdown(SocketShutdown.Both);
    //    handler.Close();
    //}
});




#endregion

while (true)
{
    if (Console.ReadLine() == "exit")
    {
        break;
    }
    else
    {
        AnsiConsole.Write(new Panel(
        Align.Center(
            new Markup("输入 [red]exit[/] 退出Bot!"),
            VerticalAlignment.Middle)));
    }
}