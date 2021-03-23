using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace HostApp
{
    public class Core
    {
        Memory Memory;

        Core() => Memory = new();

        public static Core GetInstance() => new();

        public void Run()
        {
            // first: get an image 
            Memory.CurrentImage = loadImageFile();

            // second: see if its format is allowed or supported
            Memory.CurrentImageType = loadFileType();

            // the, get its file extensio as it's on the file name
            Memory.CurrentImageExtension = loadFileExtension();

            // file verifications
            preloadChecks();

            var gCode = Memory.CurrentImage.ToBlackWhite() // then, transform the image to black and white
                                           .ToBooleanArray()      // then, create an array of booleans: black = true; white = false.
                                           .ToGCODE();            // finally, transform said boolean array into GCODE.
        }

        Image loadImageFile()
        {
            if (!File.Exists(Memory.Config.ImagePath)) throw new FileNotFoundException("Could not find image file.");
            return Bitmap.FromFile(Memory.Config.ImagePath);
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
            if (imageBytes[0..2].SequenceEqual(bmp))return Memory.Config.AllowBitmap ? ImageTypes.BMP : ImageTypes.NotAllowed;
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
