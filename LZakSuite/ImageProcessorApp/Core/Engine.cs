﻿using ImageProcessorApp.Effects;
using ImageProcessorApp.Helper;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ImageProcessorApp.Core
{
    public class Engine
    {
        Memory Memory;
        string DefaultErrorMessage = "Something went wrong! I'm sorry about that.";

        public Engine(Args ArgInfo, Config Config)
        {
            try
            {
                if (Config is null) throw new ArgumentNullException("No configuration provided.");
                if (ArgInfo is null) throw new ArgumentNullException("No parameters provided.");
                Memory = new(Config);

                if (!ArgInfo.InputFile.Exists) throw new ArgumentException($"Could not find file at {ArgInfo.InputFile.FullName}");
                Memory.CurrentImage = Image.FromFile(ArgInfo.InputFile.FullName);

                Memory.CustomOutput = ArgInfo.HasCustomOutput;
                Memory.OutputFile = ArgInfo.OutputFile;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Run()
        {
            try
            {
                Console.WriteLine("LZAK - IMAGE PIXEL MAPPING AND GCODE PROCESSOR");
                Console.WriteLine(new string('-', 46));
                Console.WriteLine();

                Console.WriteLine("- Loading image...");
                Memory.CurrentImage = loadImageFile();
                Console.WriteLine(">> Done!");

                Memory.CurrentImageType = loadFileType();
                Console.WriteLine($"- Image type: {getEnumAttribute(Memory.CurrentImageType)}");

                Console.WriteLine("- Verifying file details...");
                preloadChecks();
                Console.WriteLine(">> Done!");

                Console.WriteLine(new string('-', 3));

                var bwEffect = new SimpleBW();
                Console.WriteLine("- Applying effect \"Black and White\"...");
                var bw = Memory.CurrentImage.Apply(bwEffect);
                Console.WriteLine(">> Done!");

                Console.WriteLine("- Mapping pixels...");
                var map = bw.ToBooleanArray();
                Console.WriteLine(">> Done!");

                Console.WriteLine("- Generating GCODE...");
                var gCode = new GCODETool(Memory.Config)
                                              .ToGCODE(map);
                Console.WriteLine(">> Done!");

                Console.WriteLine("- Outputting GCODE file...");
                saveOutputFile(Memory.Config.ImagePath, gCode, out string savedFileName);
                Console.WriteLine($"-> GCODE file: \"{savedFileName}\"");
                Console.WriteLine(">> Done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(DefaultErrorMessage);
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine(new string('-', 3));
            GenericHelper.PrettyExit(5);
        }

        private string getEnumAttribute(ImageTypes currentImageType)
        {
            FieldInfo fi = Memory.CurrentImageType.GetType()
                                                  .GetField(Memory.CurrentImageType
                                                                  .ToString());

            var attr = fi.GetCustomAttributes(typeof(DescriptionAttribute), false)
                     .First() as DescriptionAttribute;

            return attr.Description;
        }
        private void saveOutputFile(string originalFile, string gCode, out string savedFileName)
        {
            var root = new FileInfo(originalFile).Name
                                                 .Split('.')
                                                 .ToArray();

            StringBuilder sb = new();
            sb.Append(string.Join("", root.SkipLast(1)
                                          .ToArray()));
            sb.Append(".g");

            savedFileName = sb.ToString();

            File.WriteAllText(savedFileName, gCode);
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
        private void preloadChecks()
        {
            if (Memory.CurrentImageType == ImageTypes.NotAllowed) throw new FileLoadException("File support has been disabled.");
            if (Memory.CurrentImageType == ImageTypes.Unsupported) throw new FileLoadException("Unsupported file type.");
        }
    }
}
