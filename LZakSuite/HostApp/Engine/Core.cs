using HostApp.Effects;
using HostApp.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace HostApp.Engine
{
    public class Core
    {
        public event EventHandler<PercentageEventArgs> Progress;

        // Contains runtime stuff and configuration instance
        Memory Memory;

        public Core(IConfig Config)
        {
            if (Config is null) throw new ArgumentNullException("No configuration provided.");
            Memory = new(Config);
        }

        public void Run()
        {
            // first: get an image 
            Memory.CurrentImage = loadImageFile();

            // second: see if its format is allowed or supported
            Memory.CurrentImageType = loadFileType();

            // the, get its file extension as it's on the file name
            Memory.CurrentImageExtension = loadFileExtension();

            // file verifications
            preloadChecks();

            // get BW image
            var bwEffct = new Effects.SimpleBW();
            bwEffct.Progress += Progress;
            var map = Memory.CurrentImage.Apply(bwEffct)    // then, transform the image to black and white
                                                            //.Apply<Effects.SimpleBW>()
                                                            //.ApplySimpleBW()
                                         .ToBooleanArray(); // then, create an array of booleans: black = true; white = false.

            var gCode = new GCODEProcessor(Memory.Config)
                                          .ToGCODE(map);    // finally, transform said boolean array into GCODE.

            gCode = gCode;
        }

        Image loadImageFile()
        {
            if (!File.Exists(Memory.Config.ImagePath)) throw new FileNotFoundException("Could not find image file.");
            return Image.FromFile(Memory.Config.ImagePath);
        }
        ImageTypes loadFileType()
        {
            var file = File.ReadAllBytes(Memory.Config.ImagePath);
            return checkFileHeaders(file);
        }
        ImageTypes checkFileHeaders(byte[] imageBytes)
        {
            // Header per supported file type
            byte[] bmp = new byte[] { 0x42, 0x4D }; // Bitmap
            byte[] jpeg = new byte[] { 0xFF, 0xD8, 0xFF }; // JPEG:
            byte[] png = new byte[] { 0x89, 0x50, 0x4E, 0x47 }; // PNG

            // If image is supported AND is allowed in config, it'll be loaded into memory.
            if (imageBytes.Length < 4) return ImageTypes.Unsupported;
            if (imageBytes[0..2].SequenceEqual(bmp)) return Memory.Config.AllowBitmap ? ImageTypes.BMP : ImageTypes.NotAllowed;
            if (imageBytes[0..3].SequenceEqual(jpeg)) return Memory.Config.AllowJPEG ? ImageTypes.JPG : ImageTypes.NotAllowed;
            if (imageBytes[0..4].SequenceEqual(png)) return Memory.Config.AllowPNG ? ImageTypes.PNG : ImageTypes.NotAllowed;

            return ImageTypes.Unsupported;
        }
        private string loadFileExtension()
        {
            return new FileInfo(Memory.Config.ImagePath).Extension;
        }
        private void preloadChecks()
        {
            if (Memory.CurrentImageType == ImageTypes.NotAllowed) throw new FileLoadException("File support has been disabled.");
            if (Memory.CurrentImageType == ImageTypes.Unsupported) throw new FileLoadException("Unsupported file type.");
        }
    }
}
