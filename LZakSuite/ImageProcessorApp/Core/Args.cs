using System.IO;

namespace ImageProcessorApp.Core
{
    public class Args
    {
        public FileInfo InputFile { get; set; }
        public FileInfo OutputFile { get; set; }
        public bool HasCustomOutput { get; set; }
    }
}
