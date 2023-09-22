using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using MineCosmos.Bot.Entity;
using MineCosmos.Bot.Service;
using Sora.Entities.Segment;
using Sora.Entities;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using MineCosmos.Bot.Helper;

namespace MineCosmos.Bot.Interactive
{
    internal class GroupFunction
    {

        public static async Task JoinTask(TaskQueueEntity model)
        {
            await SqlSugarHelper.Instance.Insertable(model).ExecuteCommandAsync();
        }

        public static string DownloadFilePath = "head_image";

        public static WeightedItem GetItem()
        {
            // 创建一个包含权重设定的列表
            List<WeightedItem> items = new List<WeightedItem>
        {
            //普通物品
            new WeightedItem { Name = "石头", Weight = 10 },
            new WeightedItem { Name = "下界岩", Weight = 9 },
            new WeightedItem { Name = "花岗岩", Weight = 9 },
            new WeightedItem { Name = "闪长岩", Weight = 8 },
            new WeightedItem { Name = "橡木", Weight = 8 },
            //中等
            new WeightedItem { Name = "方解石", Weight = 7 },
            new WeightedItem { Name = "滴水石块", Weight = 7 },
            new WeightedItem { Name = "南瓜", Weight = 7 },
            new WeightedItem { Name = "西瓜", Weight = 7 },    
            //不是很普通
            new WeightedItem { Name = "藤蔓", Weight = 6 },
            new WeightedItem { Name = "岩浆", Weight = 6 },
            new WeightedItem { Name = "水", Weight = 6 },
            new WeightedItem { Name = "雪块", Weight = 6 },
            new WeightedItem { Name = "雪球", Weight = 6 },
            new WeightedItem { Name = "可可豆", Weight = 6 },
            //普通稀有
            new WeightedItem { Name = "钻石", Weight = 5 },
            new WeightedItem { Name = "下界合金", Weight = 3 },
            //贼稀有
            new WeightedItem { Name = "苦力怕的头", Weight = 2 },
            new WeightedItem { Name = "骷髅射手头", Weight = 2 },
            new WeightedItem { Name = "骷髅射手头", Weight = 2 },
            new WeightedItem { Name = "凋零骷髅头", Weight = 2 },
            //传说级别
            new WeightedItem { Name = "信标", Weight = 1 },
            new WeightedItem { Name = "鞘翅", Weight = 1 },
            new WeightedItem { Name = "末影龙头", Weight = 1 },
            new WeightedItem { Name = "末影龙爪", Weight = 1 }
        };

            // 创建一个加权随机抽样选择器
            WeightedItemSelector selector = new WeightedItemSelector(items);
            // 调用方法获取随机结果
            WeightedItem result = selector.GetRandomWeightedItem();
            return result;
            // 输出结果
            //Console.WriteLine("Random item: " + result.Name);
        }

        public static async Task<string> DownloadQQImage(string qq)
        {
            string qqUrl = $"http://q1.qlogo.cn/g?b=qq&nk={qq}&s=100";
            //获取当前路径
            string currentPath = Path.Combine(Directory.GetCurrentDirectory(), DownloadFilePath);

            if (!Directory.Exists(currentPath))
            {
                Directory.CreateDirectory(currentPath);
            }



            // 获取文件类型
            string extension = "";
            //using (var client = new FlurlClient(qqUrl))
            using (var stream = await qqUrl.GetStreamAsync())
            using (var memStream = new MemoryStream())
            {
                await stream.CopyToAsync(memStream);
                using (var img = Image.FromStream(memStream))
                {
                    ImageFormat format = img.RawFormat;
                    if (format.Equals(ImageFormat.Gif))
                    {
                        extension = ".gif";
                    }
                    else if (format.Equals(ImageFormat.Jpeg))
                    {
                        extension = ".jpg";
                    }
                    else if (format.Equals(ImageFormat.Png))
                    {
                        extension = ".png";
                    }
                    else if (format.Equals(ImageFormat.Bmp))
                    {
                        extension = ".bmp";
                    }
                    else if (format.Equals(ImageFormat.Icon))
                    {
                        extension = ".ico";
                    }
                }
            }

            // 下载文件并保存到本地
            string fileName = $"{qq}{extension}";
            string filePath = Path.Combine(DownloadFilePath, fileName);
            await qqUrl.DownloadFileAsync(DownloadFilePath, fileName);
            return filePath;
        }

        /// <summary>
        /// 签到
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="playerInfo"></param>
        /// <returns></returns>
        public static async Task<MessageBody> SignlInAsyncMessage(int msgId, PlayerInfoEntity playerInfo)
        {
            var item = GroupFunction.GetItem();
            var integral = new Random().Next(1, 3);
            var emeraldVal = new Random().Next(5, 10);
            var luckColors = new List<string> { "红色", "黄色", "绿色", "蓝色", "紫色" };
            var luckColorndex = new Random().Next(0, luckColors.Count - 1);
            var luckNumber = new Random().Next(111, 999);
            var luckVal = new Random().Next(1, 100);

            var recordNum = await SqlSugarHelper.Instance.Queryable<PlayerSingInRecordEntity>()
               .Where(a => a.Id == playerInfo.Id)
               .CountAsync();

            await SqlSugarHelper.Instance.Insertable(new PlayerSingInRecordEntity
            {
                PlayerId = playerInfo.Id,
                CreateUserId = playerInfo.Id,
                UpdateUserId = playerInfo.Id,
                Integral = integral,
                EmeraldVal = emeraldVal,
                LuckColor = luckColors[luckColorndex],
                LuckNumber = luckNumber,
                LuckVal = luckVal,
            }).ExecuteCommandAsync();
            playerInfo.SignInCount += 1;
            playerInfo.EmeraldVal += emeraldVal;
            playerInfo.UpdateUserId = playerInfo.Id;
            await SqlSugarHelper.Instance.Updateable(playerInfo).ExecuteCommandAsync();

            var stream = ServiceCentern.commonService.GenerateImageToStream($"今日首言,自动签到成功 >_< \r\n第{recordNum}次签到", 10);
            MessageBody msg = SoraSegment.Reply(msgId) + SoraSegment.Image(stream);
            return msg;
        }


    

        public static async Task<MessageBody> GetSystemInfo(string[] values, int msgId)
        {
            var playerInfo = await SqlSugarHelper.Instance.CopyNew().Queryable<PlayerInfoEntity>().FirstAsync();

            StringBuilder sb = new();


            if (playerInfo is null)
            {
                sb.AppendLine("没有你的信息");
                return SoraSegment.Reply(msgId) + GetTextToImageMessage(sb.ToString());
            }
            else
            {
                sb.AppendLine($"名称：{playerInfo.Name}");                
                sb.AppendLine($"签到次数：{playerInfo.SignInCount}");
                sb.AppendLine($"绿宝石：{playerInfo.EmeraldVal}");
                sb.AppendLine($"创建时间：{playerInfo.CreateTime}");
                return SoraSegment.Reply(msgId) + GetTextToImageMessage(sb.ToString());
            }
        }


        private static MessageBody GetTextToImageMessage(string text)
        {
            var imgStream = ServiceCentern.commonService.GenerateImageToStream(text);
            return SoraSegment.Image(imgStream);
        }

    }
}
