namespace HostApp
{
    public class Configuration
    {
        public static Configuration Instance { get; } = new Configuration();

        public bool AllowBitmap { get; }
        public bool AllowPNG { get; }
        public bool AllowJPEG { get; }
        public string ImagePath { get; set; }

        Configuration()
        {
            AllowBitmap = true;
            AllowPNG = true;
            AllowJPEG = true;

            ImagePath = "pic.jpg";
        }
    }
}
