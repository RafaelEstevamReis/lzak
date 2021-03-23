using System.Drawing;
using System.IO;

namespace HostApp
{
    public class Core
    {
        Memory Memory = new();

        Core() { }

        public static Core GetInstance() => new();

        public void Run()
        {
            // first: get an image 
            Memory.CurrentImage = Bitmap.FromFile(Memory.Config
                                                        .ImagePath);

            // second: see if its format is allowed
            Memory.CurrentImageType = loadFileType();

            Memory = Memory;



            // then, transform the image to black and white
            // then, create an array of booleans: black = true; white = false.
            // finally, transform said boolean array into GCODE.
        }

        ImageTypes loadFileType()
        {
            // Header per supported file type
            // Bitmap: 0x42 0x4D
            // PNG: 0x89 0x50 0x4E 0x47 0x0D 0x0A 0x1A 0x0A
            // JPEG: 0xFF 0xD8 0xFF
            // If image is supported AND is allowed in config, it'll be loaded into memory.

            var file = File.ReadAllBytes(Memory.Config.ImagePath);

            // Verify if image is BMP
            if (file.Length < 2) return ImageTypes.Unsupported;
            if (file[0] == 0x42)
                if (file[1] == 0x4D)
                {
                    return Memory.Config.AllowBitmap ? ImageTypes.BMP : ImageTypes.NotAllowed;
                }

            // Verify if image is JPG
            if (file.Length < 3) return ImageTypes.Unsupported;
            if (file[0] == 0xFF)
                if (file[1] == 0xD8)
                    if (file[2] == 0xFF)
                    {
                        return Memory.Config.AllowJPEG ? ImageTypes.JPG : ImageTypes.NotAllowed;
                    }

            if (file.Length < 8) return ImageTypes.Unsupported;
            // Verify if image is PNG
            if (file[0] == 0x89)
                if (file[1] == 0x50)
                    if (file[2] == 0x4E)
                        if (file[3] == 0x47)
                            if (file[4] == 0x0D)
                                if (file[5] == 0x0A)
                                    if (file[6] == 0x1A)
                                        if (file[7] == 0x0A)
                                        {
                                            return Memory.Config.AllowPNG ? ImageTypes.PNG : ImageTypes.NotAllowed;
                                        }

            return ImageTypes.Unsupported;
        }
    }
}
