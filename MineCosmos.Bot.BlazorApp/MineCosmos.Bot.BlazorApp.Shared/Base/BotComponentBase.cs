using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting.Internal;
using MineCosmos.Bot.Common;
using MineCosmos.Bot.Entity;
using MineCosmos.Bot.Entity.Base;
using MineCosmos.Bot.Entity.Dto;
using MineCosmos.Bot.Service;
using MineCosmos.Bot.Service.Bot;

namespace MineCosmos.Bot.BlazorApp.Shared.Base
{
    /// <summary>
    /// 公共
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BotComponentBase<T> : ComponentBase
    {
        [Inject]
        [NotNull]
        public ToastService ToastService { get; set; }

        public bool DrawerIsOpen { get; set; } = false;

        public Placement DrawerAlign { get; set; } = Placement.BottomStart;

        private static long ImageMaxFileLength => 1 * 1024 * 1024;
        private static long FileMaxFileLength => 200 * 1024 * 1024;

        /// <summary>
        ///  分页数
        /// </summary>
        public static IEnumerable<int> PageItemsSource => new int[] { 10, 20, 40 };

        [NotNull]
        public List<T>? SelectedItems { get; set; } = new List<T>();

        [Inject] protected Microsoft.AspNetCore.Hosting.IWebHostEnvironment? HostEnvironment { get; set; }


        /// <summary>
        /// 服务器列表
        /// *子类需要使用时赋值
        /// </summary>
        public List<MinecraftServerEntity> ListMinecraftServerEntity { get; set; }

        public BotComponentBase() { }

        #region 常用数据集

        /// <summary>
        /// 服务器下拉列表数据
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public async  Task<QueryData<SelectedItem>> ServerDrawDropOnQueryAsync(VirtualizeQueryOption option)
        {
            var lst = ListMinecraftServerEntity
                    .Skip(option.StartIndex).Take(option.Count)
                    .Select(i => new SelectedItem(i.Id.ToString(), i.ServerName));
            return new QueryData<SelectedItem>
            {
                Items = lst.ToList(),
                TotalCount = lst.Count()
            };
        }

        #endregion

        /// <summary>
        /// 获取table查询数据
        /// </summary>
        /// <param name="pageDto"></param>
        /// <returns></returns>
        public QueryData<T> GetQueryData(ResPageDto<T> pageDto)
        {
            return new QueryData<T>()
            {
                Items = pageDto.Data,
                TotalCount = pageDto.Total,
                IsFiltered = false,
                IsSorted = false,
                IsSearch = false
            };
        }

        public QueryData<T> GetQueryData(List<T> lst)
        {
            return new QueryData<T>()
            {
                Items = lst,
                TotalCount = lst.Count,
                IsFiltered = false,
                IsSorted = false,
                IsSearch = false
            };
        }

        private async Task OnCardUpload(UploadFile file)
        {
            if (file != null && file.File != null)
            {
                // 服务器端验证当文件大于 2MB 时提示文件太大信息
                if (file.Size > ImageMaxFileLength)
                {
                    await ToastService.Information("上传图片", "图片文件大小超过1M");
                    file.Code = 1;
                    file.Error = "图片文件大小超过1M";
                }
                else
                {
                    await SaveToFile(file);
                }
            }
        }

        private CancellationTokenSource? ReadToken { get; set; }

        public string CurrentImageUrl { get; set; }

        public async Task<bool> SaveToFile(UploadFile file)
        {
            var ret = false;
            string filePath = $"/upload/images/";
            string fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{file.GetFileName()}";
            string pathToWwwroot = HostEnvironment!.WebRootPath;
            string saveFilePath = $"{pathToWwwroot}{filePath}".Replace("\\", "/");           
            string saveFileFull = Path.Combine(saveFilePath, fileName);

            ReadToken ??= new CancellationTokenSource();
            ret = await file.SaveToFileAsync(saveFileFull, ImageMaxFileLength, ReadToken.Token);
            if (ret)
            {
                file.PrevUrl = $"{AppSettings.app(new string[] { "SiteOption", "Url" })}{filePath}{fileName}";
                CurrentImageUrl = file.PrevUrl;
            }
            else
            {
                var errorMessage = $"{"保存图片失败"} {file.OriginFileName}";
                file.Code = 1;
                file.Error = errorMessage;
                await ToastService.Error("上传图片", errorMessage);
            }
            return ret;
        }


        

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            ReadToken?.Cancel();
            GC.SuppressFinalize(this);
        }

    }
}
