using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MineCosmos.Bot.BlazorApp.Shared.Base;
using MineCosmos.Bot.BlazorApp.Shared.Data;
using MineCosmos.Bot.Entity;
using MineCosmos.Bot.Service;

namespace MineCosmos.Bot.BlazorApp.Shared.Pages
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Users : BotComponentBase<PlayerInfoEntity>
    {
        [Inject]
        [NotNull]
        private IPlayerService playerService { get; set; }

        private async Task<QueryData<PlayerInfoEntity>> OnQueryAsync(QueryPageOptions options)
        {
            var pageInfo = await playerService.PagePlayer(new Entity.Dto.ReqPageDto
            {
                Key = options.SearchText??"",
                PageNum = options.PageIndex,
                PageSize = options.PageItems
            });
            return base.GetQueryData(pageInfo);
        }
    }
}