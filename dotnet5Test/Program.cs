using System;
using System.IO;
using SkiaSharp;

namespace dotnet5Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            float x1 = 12f;

           

            Console.WriteLine(x1.ToString("0.00").Split('.')[1]);

            Stream stream = CreateProductQrImage("prod.png", "qrCode.png", "99.00", "99", "八周年秒杀 | 姬霓太美 | 好好好，这样起标题是吧 | 🚛🚒");

            using FileStream fileStream = File.Create("genderImage.png");
            stream.CopyTo(fileStream);
        }
        public static Stream CreateProductQrImage(string prodPath, string qrCodePath, string regularPrice, string salePrice, string prodTitle, int marginX = 10, int marginY = 10, int additionalH = 200, string fontPath = "font.ttf")
        {

            SKBitmap prodBitmap = SKBitmap.Decode(prodPath);


            //根据产品图片大小生成画布大小
            using SKBitmap mainBitmap = new SKBitmap(prodBitmap.Width + marginX * 2, prodBitmap.Height + additionalH + marginY * 2);

            using SKCanvas mainCanvas = new SKCanvas(mainBitmap);
            mainCanvas.Clear();
            mainCanvas.DrawColor(SKColors.White);

            //画产品图
            mainCanvas.DrawBitmap(prodBitmap, x: marginX, y: marginY);

            //画文字 MeasureText

            var line_1Paint = GetSKPaint(fontPath, SKColor.Parse("#af243d"));
            string row1_col1Text = "￥";
            string row1_col2Text = salePrice;
            string row1_col3Text = ".00";


            SKRect row1_col1GapSkRect = new SKRect();
            SKRect row1_col2GapSkRect = new SKRect();
            SKRect row2_GapSkRect = new SKRect();
            line_1Paint.MeasureText(text: row1_col1Text, bounds: ref row1_col1GapSkRect);

            float row1Y = prodBitmap.Height + prodBitmap.Height / 9;
            mainCanvas.DrawText(row1_col1Text, marginX, row1Y, line_1Paint);

            float row1X = marginX + row1_col1GapSkRect.Width + row1_col1GapSkRect.Width / 2;
            line_1Paint.TextSize = 45F;
            mainCanvas.DrawText(row1_col2Text, row1X, y: row1Y, line_1Paint);


            line_1Paint.MeasureText($"{row1_col2Text}", ref row1_col2GapSkRect);
            line_1Paint.TextSize = 25F;
            row1X = marginX + row1_col2GapSkRect.Width + (row1_col2GapSkRect.Width / 2) + (row1_col2GapSkRect.Width / 7);
            mainCanvas.DrawText(row1_col3Text, row1X, y: row1Y, line_1Paint);

            row1Y = row1Y + row1_col1GapSkRect.Height * 2 + 10;
            string line_2Text = $"价格 {regularPrice}";
            var line_2Patin = GetSKPaint(fontPath, SKColors.Gray, 15F, false);
            mainCanvas.DrawText(line_2Text, marginX, row1Y, line_2Patin);
            line_2Patin.MeasureText(line_2Text, ref row2_GapSkRect);

            //画线
            SKPaint linePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Gray,
                StrokeWidth = 2
            };
            float lineY = row1Y - row2_GapSkRect.Height * .4f;
            mainCanvas.DrawLine(marginX, lineY, marginX + row2_GapSkRect.Width, lineY, linePaint);

            DrawMuiltLineText(
                mainCanvas,
                prodTitle,
                marginX,
                row1Y + row2_GapSkRect.Height + 10,
                GetSKPaint(fontPath, SKColor.Parse("#292929"), 25F, false, 1),
                prodBitmap.Width / 2 + 10,
                2);


            using SKBitmap qrCodeBimap = SKBitmap.Decode(qrCodePath);
            int wh = 150;
            int x = mainBitmap.Width - wh - wh / 5;
            int y = mainBitmap.Height - wh - wh / 5;
            //画二维码
            //using SKImage qrCodeSKImage = SKImage.FromEncodedData(SKData.Create(qrCodePath));

            //int x = mainBitmap.Width - 100; //- qrCodeSKImage.Width;
            //int y = mainBitmap.Height - 100;// - qrCodeSKImage.Height;


            SKRect qrCodeRect = SKRect.Create(x, y, wh, wh);
            //绘制头像
            mainCanvas.DrawBitmap(qrCodeBimap, qrCodeRect);


            Stream stream = SKImage.FromBitmap(mainBitmap).Encode(SKEncodedImageFormat.Png, 100).AsStream();
            return stream;
        }

        static SKPaint GetSKPaint(string fontPath, SKColor sKColors, float size = 24F, bool bold = true, float scale = 1f)
        {
            using FileStream fs = File.OpenRead(fontPath);
            SKTypeface Font = SKTypeface.FromStream(fs);

            //字体设置
            SKPaint text_paint = new SKPaint
            {
                TextSize = 24F,
                Color = sKColors,
                Typeface = Font,
                TextScaleX = scale,
                FakeBoldText = bold,
                IsAntialias = true,
                IsDither = true
            };

            return text_paint;
        }

        /// </summary>
        /// <param name="canvas">画布</param>
        /// <param name="text">多行文字</param>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="paint">画笔</param>
        /// <param name="maxWidth">单行文字最大宽度</param>
        /// <param name="maxLines">最大行数，如果超出，则使用...</param>
        public static void DrawMuiltLineText(SKCanvas canvas, string text, float x, float y, SKPaint paint, float maxWidth, int maxLines)
        {
            //已写出行数
            int lines = 0;
            //省略号宽度
            var ellipseWidth = paint.MeasureText("...");
            //如果文字没写完且已写行数小于最大行数，则继续写出
            while (!string.IsNullOrEmpty(text) && lines < maxLines)
            {
                //单行文字
                string lineStr;
                //单行文字长度
                long strLength;
                //非最后一行
                if (lines != maxLines - 1)
                {
                    //调用BreakText方法，可计算指定宽度能写出多少文字
                    strLength = paint.BreakText(text, maxWidth, out float ww, out lineStr);
                }
                //最后一行
                else
                {
                    //调用BreakText方法计算单行文字长度，这里需减去省略号宽度
                    strLength = paint.BreakText(text, maxWidth - ellipseWidth, out float ww, out lineStr);
                    //假如字还没写完，需加省略号
                    if (strLength < text.Length)
                    {
                        lineStr += "...";
                    }
                }
                //文字矩形
                var tRect = new SKRect();
                //计算文字矩形
                paint.MeasureText(lineStr, ref tRect);
                //计算坐标
                var tPoint = new SKPoint
                {
                    X = x - tRect.Left,
                    //这里注意Y坐标需要加上行高
                    Y = y + lines * paint.FontSpacing - tRect.Top
                };
                //写出一行文字
                canvas.DrawText(lineStr, tPoint, paint);
                //已写出行数加1
                lines++;

                //取剩余文字
                text = text.Substring((int)strLength);
            }
        }

    }
}
