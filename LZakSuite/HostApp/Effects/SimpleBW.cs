using HostApp.Interfaces;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace HostApp.Effects
{
    public class SimpleBW : IEffect
    {
        public Image Process(Image image)
        {
            applyTransformation(image, c =>
            {
                byte gray = (byte)((c.R + c.G + c.B) / 3 > 127 ? 255 : 0); // WTF?
                return Color.FromArgb(gray, gray, gray);
            });

            // for debug purposes
            saveOutputToFile(image);

            return image;
        }
        private void saveOutputToFile(Image image)
        {
            using var fs = File.OpenWrite("out.jpg");
            image.Save(fs, ImageFormat.Jpeg);
        }
        void applyTransformation(Image image, Func<Color, Color> action)
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

    public static class SimpleBWExtensions
    {
        public static Image ApplySimpleBW(this Image img)
        {
            var bw = new SimpleBW();
            return bw.Process(img);
        }
    }
}
