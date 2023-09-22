using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace MineCosmos.Bot.BlazorApp.Shared.Pages
{
    public partial class Index
    {
        [NotNull]
        private Chart? PieChart { get; set; }

        [Inject]
        [NotNull]
        private WebClientService? ClientService { get; set; }
        private ClientInfo ClientInfo { get; set; } = new ClientInfo();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                ClientInfo = await ClientService.GetClientInfo();
                StateHasChanged();
            }
        }



        private Task<ChartDataSource> OnInit()
        {
            var ds = new ChartDataSource();
            ds.Options.Title = "指令使用统计图";
            ds.Labels = new List<string> { "/tp", "/qd", "/bind" };
            var lstData = Enumerable.Range(1, ds.Labels.Count()).Select(i => new Random().Next(20, 37)).Cast<object>();
            ds.Data.Add(new ChartDataset()
            {
                Label = $"指令使用次数",
                Data = lstData
            });


            return Task.FromResult(ds);
        }

        private Task OnAfterInit()
        {
            return Task.CompletedTask;
        }

        private Task OnAfterUpdate(ChartAction action)
        {
            return Task.CompletedTask;
        }
    }
}
