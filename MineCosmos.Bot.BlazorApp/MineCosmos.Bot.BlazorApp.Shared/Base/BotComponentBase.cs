using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
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
        public bool DrawerIsOpen { get; set; } = false;

        public Placement DrawerAlign { get; set; } = Placement.BottomStart;

        /// <summary>
        ///  分页数
        /// </summary>
        public static IEnumerable<int> PageItemsSource => new int[] { 10, 20, 40 };

        [NotNull]
        public List<T>? SelectedItems { get; set; } = new List<T>();
       


        public BotComponentBase() { }

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

    }
}
