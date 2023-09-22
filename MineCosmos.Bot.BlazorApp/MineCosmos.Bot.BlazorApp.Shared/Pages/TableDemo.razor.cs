using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Localization;
using MineCosmos.Bot.BlazorApp.Shared.Base;
using MineCosmos.Bot.BlazorApp.Shared.Data;
using MineCosmos.Bot.Entity;
using MineCosmos.Bot.Entity.Dto;
using MineCosmos.Bot.Service.Bot;

namespace MineCosmos.Bot.BlazorApp.Shared.Pages
{
    /// <summary>
    /// 
    /// </summary>
    public partial class TableDemo : BotComponentBase<MinecraftServerEntity>
    {
        [Inject]
        [NotNull]
        private IServerManagerService? serverManagerService { get; set; }

        private int ServerId { get; set; } = 100;
        private string Comand { get; set; } = "/list";

        /// <summary>
        /// OnInitialized
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private async Task<QueryData<MinecraftServerEntity>> OnQueryAsync(QueryPageOptions options)
        {
            var queryPageInfo =
                await serverManagerService.PageServers(new() { PageNum = options.PageIndex, PageSize = options.PageItems });
            return GetQueryData(queryPageInfo);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <param name="changedType"></param>
        /// <returns></returns>
        private async Task<bool> OnSaveAsync(MinecraftServerEntity model, ItemChangedType changedType)
        => await serverManagerService.SaveServer(model);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<bool> OnDeleteAsync(IEnumerable<MinecraftServerEntity> model) => await serverManagerService.Remove<MinecraftServerEntity>(a => model.Select(a=>a.Id).Contains(a.Id));

     
    }
}