using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MineCosmos.Bot.BlazorApp.Shared.Base;
using MineCosmos.Bot.BlazorApp.Shared.Data;
using MineCosmos.Bot.Entity;
using MineCosmos.Bot.Entity.Base;
using MineCosmos.Bot.Entity.Dto;
using MineCosmos.Bot.Service;

namespace MineCosmos.Bot.BlazorApp.Shared.Pages
{
    /// <summary>
    ///  指令
    /// </summary>
    public partial class Comand : BotComponentBase<CommandGroupEntity>
    {
        /// <summary>
        /// 玩家服务
        /// </summary>
        [Inject]
        [NotNull]
        private IPlayerService? playerService { get; set; }

        /// <summary>
        /// 服务器服务
        /// </summary>
        [Inject]
        [NotNull]
        private IServerManagerService? serverManagerService { get; set; }

        /// <summary>
        /// 指令服务
        /// </summary>
        [Inject]
        [NotNull]
        private ICommandManagerService? CommandManagerService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await Task.CompletedTask;
        }


        private async Task<QueryData<CommandGroupEntity>> OnQueryAsync(QueryPageOptions options)
        {
            var pageModel = new ReqPageDto
            {
                Key = options.SearchText ?? "",
                PageNum = options.PageIndex,
                PageSize = options.PageItems
            };

            //这里为了展示多种使用方式，就没有封装进service了
            var pageData = await CommandManagerService.GetIncludesPageDataAsync<CommandGroupEntity>(pageModel, a => 
            ( pageModel.Key == "" || a.Name.Contains(pageModel.Key))
            );

            base.ListMinecraftServerEntity = await serverManagerService.GetListServer();
            return base.GetQueryData(pageData);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <param name="changedType"></param>
        /// <returns></returns>
        private async Task<bool> OnSaveAsync(CommandGroupEntity model, ItemChangedType changedType)
        {
            return await CommandManagerService.SaveCommandGroup(model);
        }

    }
}