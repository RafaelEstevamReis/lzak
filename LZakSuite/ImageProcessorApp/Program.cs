using HostApp.Core;
using System;
using System.IO;

namespace HostApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            var file = GetFileFromArgs(args);

            var c = new Engine(file, new Config());
            c.Run();
        }

        private static FileInfo GetFileFromArgs(string[] args)
        {
            if (args is null) throw new ArgumentException("No file path provided.");
            if(args.Length == 0) throw new ArgumentException("You must provide a valid image file path.");

            try { return new FileInfo(args[0]); }
            catch { throw new ArgumentException("Invalid file name."); }
        }
    }
}
