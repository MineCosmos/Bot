using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace MineCosmos.Bot.BlazorApp.Shared.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class MainLayout
    {
        private bool UseTabSet { get; set; } = false;

        private string Theme { get; set; } = "color1";

        private bool IsOpen { get; set; }

        private bool IsFixedHeader { get; set; } = true;

        private bool IsFixedFooter { get; set; } = true;

        private bool IsFullSide { get; set; } = true;

        private bool ShowFooter { get; set; } = false;

        private List<MenuItem>? Menus { get; set; }

        /// <summary>
        /// OnInitialized 方法
        /// </summary>
        protected async override Task OnInitializedAsync()
        {
            base.OnInitialized();

            Menus = GetIconSideMenuItems();
        }

        private static List<MenuItem> GetIconSideMenuItems()
        {
            var menus = new List<MenuItem>
        {
            new MenuItem() { Text = "首页", Icon = "fa-solid fa-fw fa-home", Url = "/" , Match = NavLinkMatch.All},
              new MenuItem() { Text = "用户管理", Icon = "fa-solid fas fa-users-viewfinder", Url = "users" },
            new MenuItem() { Text = "指令管理", Icon = "fa-solid fas fa-server", Url = "/comand" },            
            new MenuItem() { Text = "服务器管理", Icon = "fa-solid fas fa-code", Url = "servermanage" },
            new MenuItem() { Text = "数据设置", Icon = "fa-solid fa-fw fa-database", Url = "fetchdata" }          
        };

            return menus;
        }
    }
}