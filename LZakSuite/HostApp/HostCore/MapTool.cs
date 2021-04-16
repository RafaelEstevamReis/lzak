using System.Drawing;

namespace HostApp.HostCore
{
    public static class MapTool
    {
        public static bool[,] ToBooleanArray(this System.Drawing.Image image)
        {
            bool[,] array = new bool[image.Width, image.Height];
            var bmp = (Bitmap)image;

            for (int width = 0; width < image.Width; width++)
            {
                for (int height = 0; height < image.Height; height++)
                {
                    var c = bmp.GetPixel(width, height);

                    bool result = c.R > 127 || c.G > 127 || c.B > 127;
                    array[width, height] = result;
                }
            }

            return array;
        }
    }
}
