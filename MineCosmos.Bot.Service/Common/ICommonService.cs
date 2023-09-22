using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineCosmos.Bot.Service.Common
{
    /// <summary>
    /// 通用服务接口
    /// </summary>
    public interface ICommonService: IBaseService
    {
        /// <summary>
        /// 下载qq头像到本地
        /// </summary>
        /// <param name="qq"></param>
        /// <returns></returns>
        Task<string> DownloadQQAvator(string qq);

        /// <summary>
        /// 生成图片流
        /// </summary>
        /// <param name="text"></param>
        /// <param name="marginTop"></param>
        /// <param name="marginBottom"></param>
        /// <param name="marginLeft"></param>
        /// <param name="marginRight"></param>
        /// <returns></returns>
        Stream GenerateImageToStream(string text, int marginTop = 10, int marginBottom = 5, int marginLeft = 10, int marginRight = 10);
    }
}
