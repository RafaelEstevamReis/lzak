using System.Drawing;

namespace HostApp.Core
{
    public class Memory
    {
        public Config Config { get; }
        public Image CurrentImage { get; set; }
        public ImageTypes CurrentImageType { get; set; }
        public Memory(Config Config) => this.Config = Config;
    }
}
