using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MineCosmos.Bot.Entity;

namespace MineCosmos.Bot.Service.Bot
{
    public interface IBotService: IBaseService
    {
        Task<PlayerInfoEntity> CreateNewPlayer(PlayerInfoEntity entity);
        Task<CommandModel> GetCommandFromMessage(string msgText);
        Task<PlayerInfoEntity> GetPlayerInfoByPlatformId(string platformId);
        Task<PlayerInfoEntity> GetPlayerInfoByPlatformName(string platformName);
        Task<Stream?> SingInByPlayerId(int playerId);
        Task<Stream?> SpeechRecordByPlayerId(string platformId, string platformName, string message);
    }
}
