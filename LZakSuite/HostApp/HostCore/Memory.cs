namespace HostApp.HostCore
{
    public class Memory
    {
        public Config Config { get; }
        public System.Drawing.Image CurrentImage { get; set; }
        public ImageTypes CurrentImageType { get; set; }
        public string CurrentImageExtension { get; set; }
        public Memory(Config Config) => this.Config = Config;
    }
}
