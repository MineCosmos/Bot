using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;

namespace MineCosmos.Bot.Service.Bot
{
   
    //TODO 改名
    public class CommandHandleService : BaseService, ICommandHandleService
    {
        readonly IBotService _botService;
        public CommandHandleService(IBotService botService)
        {
            _botService = botService;
        }

        /// <summary>
        /// 获取UUID
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<UUIDModel?> GetMinecraftUUID(string message)
        {
            var commandInfo = await _botService.GetCommandFromMessage(message);

            if (commandInfo.Commands.Length < 2 || commandInfo.Commands[1].Equals(string.IsNullOrWhiteSpace))            
                return null;
            
            string name = commandInfo.Commands[1];

            string url = $"https://api.mojang.com/users/profiles/minecraft/{name}";
            var uuidInfo = await url.GetAsync().ReceiveJson<UUIDModel>();
            return uuidInfo;
        }

        /// <summary>
        /// 根据UUID获取皮肤头像
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public async Task<Stream> GetMinecraftSkin(string uuid)
        {
            //var skinUrl = $"https://crafatar.com/skins/{uuid}";
            var skinUrl = $"https://crafatar.com/avatars/{uuid}";
            var skinStream = await skinUrl.GetStreamAsync();
            return skinStream;
        }
    }

    public class UUIDModel
    {
        public string id { get; set; }
        public string name { get; set; }
    }

   
}
