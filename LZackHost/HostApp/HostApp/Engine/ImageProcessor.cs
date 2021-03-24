using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace HostApp
{
    public class PercentageEventArgs
    {
        public double Progress { get; set; }
        public PercentageEventArgs(double Progress) => this.Progress = Progress;
    }

    public delegate void onProgressChange(object sender, PercentageEventArgs e);

    public static class ImageProcessor
    {
        public static event onProgressChange ReportPercentage;

        public static Image ToBlackAndWhite(this Image image)
        {
            applyEffect(image, c =>
            {
                byte gray = (byte)((c.R + c.G + c.B) / 3 > 127 ? 255 : 0); // WTF?
                return Color.FromArgb(gray, gray, gray);
            });

            using MemoryStream ms = new();
            image.Save(ms, ImageFormat.Jpeg);
            using FileStream fs = File.Create("out.jpg");
            fs.Write(ms.ToArray());

            return image;
        }
        static void applyEffect(Image image, Func<Color, Color> action)
        {
            var bmp = (Bitmap)image;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var c = bmp.GetPixel(x, y);
                    c = action(c);
                    bmp.SetPixel(x, y, c);
                }
            }
        }
    }
}
