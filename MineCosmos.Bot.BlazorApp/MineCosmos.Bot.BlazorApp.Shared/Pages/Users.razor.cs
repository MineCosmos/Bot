using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MineCosmos.Bot.BlazorApp.Shared.Base;
using MineCosmos.Bot.BlazorApp.Shared.Data;
using MineCosmos.Bot.Entity;
using MineCosmos.Bot.Entity.Dto;
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
        private IPlayerService? playerService { get; set; }

        [Inject]
        [NotNull]
        private IServerManagerService? serverManagerService { get; set; }


        private SelectedItem? ServerDropDownSelectItem { get; set; } = new();
        private List<UploadFile> PreviewFileList { get; set; }

        /// <summary>
        /// 点击编辑按钮时
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task OnEditAsync(PlayerInfoEntity model)
        {
            //图片手动赋值
            if (!string.IsNullOrWhiteSpace(model.Avatar))
            {
                PreviewFileList = new() { new UploadFile { PrevUrl = model.Avatar } };
                CurrentImageUrl = model.Avatar;
            }

            //下拉框手动赋值
            if (model.ServerInfo is null) return;
            ServerDropDownSelectItem.Text = model.ServerInfo.ServerName;
            ServerDropDownSelectItem.Value = model.ServerId.ToString();
        }

        private async Task<QueryData<PlayerInfoEntity>> OnQueryAsync(QueryPageOptions options)
        {
            ResPageDto<PlayerInfoEntity>? pageInfo = await playerService.PagePlayer(new ReqPageDto
            {
                Key = options.SearchText ?? "",
                PageNum = options.PageIndex,
                PageSize = options.PageItems
            });
            base.ListMinecraftServerEntity = await serverManagerService.GetListServer();
            return base.GetQueryData(pageInfo);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <param name="changedType"></param>
        /// <returns></returns>
        //private async Task<bool> OnSaveAsync(PlayerInfoEntity model, ItemChangedType changedType)
        //=> await playerService.SavePlayer(model);

        private async Task<bool> OnSaveAsync(PlayerInfoEntity model, ItemChangedType changedType)
        {
            //model.ServerId = CurrentSelectServerId;

            model.Avatar = CurrentImageUrl;
            return await playerService.SavePlayer(model);
        }

        //private async Task<QueryData<SelectedItem>> ServerDrawDropOnQueryAsync(VirtualizeQueryOption option)
        //{
        //    var lst = await serverManagerService.GetListServer(option.SearchText ?? "");
        //    return new QueryData<SelectedItem>
        //    {
        //        Items = lst
        //            .Skip(option.StartIndex).Take(option.Count)
        //            .Select(i => new SelectedItem(i.Id.ToString(), i.ServerName)),
        //        TotalCount = lst.Count
        //    };
        //}
        //serverChoose
    }
}