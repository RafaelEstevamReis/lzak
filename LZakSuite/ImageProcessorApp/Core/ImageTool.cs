using System;
using System.Drawing;

namespace ImageProcessorApp.Core
{

    public class ImageTool
    {
        public Image ToBlackAndWhite(Image image)
        {
            applyTransformation(image, c =>
            {
                byte gray = (byte)((c.R + c.G + c.B) / 3 > 127 ? 255 : 0); // WTF?
                return Color.FromArgb(gray, gray, gray);
            });

            return image;
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
}
