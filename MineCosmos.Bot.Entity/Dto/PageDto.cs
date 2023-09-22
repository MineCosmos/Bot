#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineCosmos.Bot.Entity.Dto;

/// <summary>
/// 请求分页数据传输对象
/// </summary>
public class ReqPageDto
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageNum { get; set; }

    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; }
}

/// <summary>
/// 响应分页数据传输对象
/// </summary>
/// <typeparam name="TData">数据类型</typeparam>
public class ResPageDto<TData>
{
    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPage { get; set; }

    /// <summary>
    /// 总记录数
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 页码
    /// </summary>
    public int PageNum { get; set; }

    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 数据
    /// </summary>
    public List<TData> Data { get; set; }
}

