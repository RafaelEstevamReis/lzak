using HostApp.Interfaces;
using System.Drawing;

namespace HostApp.HostCore
{
    public class Memory
    {
        public IHostConfig Config { get; }
        public Image CurrentImage { get; set; }
        public ImageTypes CurrentImageType { get; set; }
        public string CurrentImageExtension { get; set; }

        public Memory(IHostConfig Config) => this.Config = Config;
    }
}
