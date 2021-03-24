using System.Drawing;

namespace HostApp
{
    public static class MapProcessor
    {
        public static bool[,] ToBooleanArray(this Image image)
        {
            bool[,] array = new bool[image.Width, image.Height];
            var bmp = (Bitmap)image;

            for (int width = 0; width < image.Width; width++)
            {
                for (int height = 0; height < image.Height; height++)
                {
                    var c = bmp.GetPixel(width, height);

                    if (c.R == 255 || c.G == 255 || c.B == 255)
                    {
                        array[width, height] = true;
                        continue;
                    }
                }
            }

            return array;
        }
    }
}
