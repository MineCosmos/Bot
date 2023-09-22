using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace MineCosmos.Common
{
    public class ImageHelper
    {
        /// <summary>
        /// 自定义字体
        /// </summary>
        static SKTypeface Font;

        /// <summary>
        /// 生成活动海报
        /// </summary>
        /// <param name="bgPath">背景地址</param>        
        /// <param name="qrPath">二维码地址</param>
		/// <param name="coverPath">头像地址</param>
		/// <param name="fontSize">字体大小</param>
        /// <param name="title">标题</param>
        /// <param name="fontNameSize">昵称字体大小</param>
        /// <param name="fontNameColor">昵称字体颜色</param>
        public static Stream CreateActivityDiyQrCode(string bgPath, string qrPath, string coverPath, string title, float fontNameSize = 24F, string fontNameColor = "#ffffff", float fontSize = 24F)
        {
            if (!File.Exists(bgPath))
                throw new Exception($"不存在文件：{bgPath}");
            if (Font == null)
            {
                var fontPath = $"LXGWWenKaiGBScreenR.ttf";
                using (FileStream fs = File.OpenRead(fontPath))
                {
                    Font = SKTypeface.FromStream(fs);
                }
            }
            //字体设置
            var text_paint = new SKPaint
            {
                TextSize = fontSize,
                Typeface = Font,
                FakeBoldText = true,
                IsAntialias = true,
                IsDither = true
            };
            // 创建一个主画布
            using var bgMap = SKBitmap.Decode(bgPath);
            var mapInfo = new SKImageInfo(495, 880);
            var map = SKBitmap.FromImage(SKImage.Create(mapInfo));
            // 创建一个主画布的画布上下文
            using var mainCanvas = new SKCanvas(map);
            var bsSize = new SKSizeI(495, 880);
            var bgImage = bgMap.Resize(bsSize, SKFilterQuality.High);
            mainCanvas.DrawColor(SKColors.White);
            mainCanvas.DrawBitmap(bgImage, 0, 0);

            if (!string.IsNullOrWhiteSpace(coverPath))
            {
                //头像画布
                //int coverW = 48, coverH = 48;
                int coverW = 60, coverH = 60;
                SKRoundRect coverRoundRect = new SKRoundRect(new SKRect(0, 0, coverW, coverH), coverW / 2, coverH / 2);
                var coverInfo = NewMapHandle(coverW, coverH, coverRoundRect, coverPath, coverW, coverH, 0, 0, SKColors.Transparent);
                using var coverMap = coverInfo.Item1;
                mainCanvas.DrawBitmap(coverMap, 16, y: 24);
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                var nameColor = SKColor.TryParse(fontNameColor, out var color) ? color : SKColors.Black;
                //写名字
                var text_name_paint = new SKPaint
                {
                    TextSize = fontNameSize,
                    Color = nameColor,
                    Typeface = Font,
                    FakeBoldText = true,
                    IsAntialias = true,
                    IsDither = true
                };
                mainCanvas.DrawText(title, 72, 53, text_name_paint);
            }

            //二维码画布
            var childSurface1Height = mapInfo.Height - (630 + 20);
            var childSurface1Width = mapInfo.Width - (283 + 20);
            SKRoundRect roundRect = new SKRoundRect(new SKRect(0, 0, childSurface1Width, childSurface1Height), 11, 11);
            var qrInfo = NewMapHandle(childSurface1Width, childSurface1Height, roundRect, qrPath, 160, 160, 16, 12, SKColors.White);
            using var qrMap = qrInfo.Item1;
            using var qrCanvas = qrInfo.Item2;
            var qrTextY = 200;
            var qrTextX = 35;

            text_paint.TextSize = 25F;
            qrCanvas.DrawText("扫码加好友", qrTextX, qrTextY, text_paint);

            // 把二维码画布画到主画布上
            mainCanvas.DrawBitmap(qrMap, 283, 630);
            var stream = SKImage.FromBitmap(map).Encode(SKEncodedImageFormat.Png, 100).AsStream();

            return stream;
        }

        private static (SKBitmap, SKCanvas) NewMapHandle(int w, int h, SKRoundRect rect, string sysImgPath, int reSetW, int reSetH, int resetMapOnCanvasX, int resetMapOnCanvasY, SKColor fillSKColors)
        {
            var newMap = new SKBitmap(w, h);
            var newMapCanvas = new SKCanvas(newMap);
            newMapCanvas.Clear(SKColors.Transparent);
            newMapCanvas.ClipRoundRect(rect, SKClipOperation.Intersect, true);

            var sysImgMap = SKBitmap.Decode(sysImgPath);
            var sysImgResetMap = sysImgMap.Resize(new SKSizeI(reSetW, reSetH), SKFilterQuality.High);
            newMapCanvas.DrawColor(fillSKColors);
            newMapCanvas.DrawBitmap(sysImgResetMap, resetMapOnCanvasX, resetMapOnCanvasY);

            return (newMap, newMapCanvas);
        }
    }
}
