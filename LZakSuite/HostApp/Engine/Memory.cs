using HostApp.Interfaces;
using System.Drawing;

namespace HostApp.Engine
{
    public class Memory
    {
        public IConfig Config { get; }
        public Image CurrentImage { get; set; }
        public ImageTypes CurrentImageType { get; set; }
        public string CurrentImageExtension { get; set; }

        public Memory(IConfig Config) => this.Config = Config;
    }
}
