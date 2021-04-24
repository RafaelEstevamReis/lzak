using System.Drawing;
using System.IO;

namespace ImageProcessorApp.Core
{
    public class Memory
    {
        public Config Config { get; }
        public Image CurrentImage { get; set; }
        public ImageTypes CurrentImageType { get; set; }
        public bool CustomOutput { get; internal set; }
        public FileInfo OutputFile { get; internal set; }
        public FileInfo CurrentImageFileDetails { get; internal set; }

        public Memory(Config Config) => this.Config = Config;
    }
}
