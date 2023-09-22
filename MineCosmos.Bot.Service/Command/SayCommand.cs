using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kook.WebSocket;
using MineCosmos.Bot.Entity;
using MineCosmos.Bot.Service;
using MineCosmos.Bot.Service.Bot;
using Minecraft.Messages;

namespace MineCosmos.Bot.Service.Command;
public class SayCommand : BaseCommand, ICommand
    {
        public SayCommand(
            CommandData data)
        {
            _data = data;
        }



        public async Task<CommandExcuteResultModel> Execute()
        {
            string onlineText = "离线";
            //if (await CheckOnline(sayUser.Name)) onlineText = "在线";

            var sayUser = await SqlSugarHelper.Instance.Queryable<PlayerInfoEntity>().FirstAsync(a => a.KookUserId == _data.SocketGuildUser.Id.ToString());
            if (sayUser == null)
            {
                //TODO 错误处理
                return Error("你还未绑定账号，无法使用say功能");
            }

            MessageComponent titleCcomponent = MessageComponent.Create()
            .WithText($"[KOOK] ")
            .WithColor(MinecraftColor.Green)
            .WithFont("minecraft:default")
            .WithClickEvent(ClickAction.OpenUrl, "https://www.kookapp.cn/app/invite/CQ0ojJ")
            .WithHoverEvent(HoverAction.ShowText, $"[{_data.SocketMessage.Channel.Name}频道] [点击加入]");

            MessageComponent statusCcomponent = MessageComponent.Create()
                  .WithText($"[{onlineText}]")
                  .WithColor(MinecraftColor.Gray)
                  .WithFont("minecraft:default");

            MessageComponent senderCcomponent = MessageComponent.Create()
                    .WithText($"[{_data.SocketMessage.Timestamp.ToString("HH:mm")}] <{sayUser.Name}>")
                    .WithColor(MinecraftColor.White)
                    .WithFont("minecraft:default")
                    .WithHoverEvent(HoverAction.ShowText, $"{sayUser.Name}");

            MessageComponent contentCcomponent = MessageComponent.Create()
                    .WithText($" {GetCommandArry()[0]}")
                    .WithColor(MinecraftColor.White)
                    .WithFont("minecraft:default")
                    .WithHoverEvent(HoverAction.ShowText, $"想要回复消息？来KOOK频道！");
            JsonMessageBuilder builder1 = new();
            builder1.AddComponent(titleCcomponent)
                    .AddComponent(statusCcomponent)
                    .AddComponent(senderCcomponent)
                    .AddComponent(contentCcomponent);
            string json = builder1.ToJson();
            string mcCommand = $"tellraw @a {json}";

            var sayRes = await _data.ServerManager!.SendAsync(_data.CurrentMcServer.Id, mcCommand);

            return Success();
        }
    }

