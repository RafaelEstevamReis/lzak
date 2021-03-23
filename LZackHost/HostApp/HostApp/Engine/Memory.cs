using System.Drawing;

namespace HostApp
{
    public class Memory
    {
        public static Configuration Config { get; } = Configuration.Instance;
        public Image CurrentImage { get; set; }
        public ImageTypes CurrentImageType { get; set; }
        public string CurrentImageExtension { get; set; }

        public Memory()
        {

        }
    }
}
