#nullable disable
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

namespace MineCosmos.Bot.BlazorApp.Shared.Components;
/// <summary>
/// 
/// </summary>
public partial class MinecraftCodeDropDown: BotComponentBase<CommandGroupEntity>
{
    [Inject]
    [NotNull]
    public ICommandManagerService commandManagerService { get; set; }

    private SelectedItem VirtualItem1 { get; set; }

    [Parameter]
    public EventCallback<int> OnSelectItemChanged { get; set; }


    [NotNull]
    [Parameter]
    public int McServerId { get; set; }

    private async Task OnSelectedItemChanged()
    {
        if (OnSelectItemChanged.HasDelegate)
        {
            int.TryParse(VirtualItem1.Value, out int selectServerId);
            await OnSelectItemChanged.InvokeAsync(selectServerId);
        }
    }

    private async Task<QueryData<SelectedItem>> OnQueryAsync(VirtualizeQueryOption option)
    {

        var lst = await commandManagerService.GetListCommandGroupByServerId(McServerId);
      
        if (!string.IsNullOrEmpty(option.SearchText))
        {
            lst = lst.Where(i => i.Name!.Contains(option.SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return new QueryData<SelectedItem>
        {
            Items = lst
            .Skip(option.StartIndex).Take(option.Count)
            .Select(i => new SelectedItem(i.Id.ToString(), i.Name)),
            TotalCount = lst.Count
        };
    }



}
