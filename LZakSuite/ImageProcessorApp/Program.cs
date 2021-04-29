using ImageProcessorApp.Core;
using System;
using System.IO;
using System.Text;

namespace ImageProcessorApp
{
    partial class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var argObj = getArgs(args);
                if (argObj is null) return;

                var c = new Engine(argObj, new Config());
                c.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No way! Reason: {ex.Message}");
            }
        }

        private static Args getArgs(string[] args)
        {
            string inputPath = string.Empty;
            string outputPath = string.Empty;
            bool customOutput = false;


            if (args is null) throw new ArgumentException("No file path provided.");
            if (args.Length == 0)
            {
                printHelp();
                return null;
            }

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-i":
                        if (args[i + 1] is null)
                        {
                            inputPath = string.Empty;
                            break;
                        }
                        inputPath = args[i + 1];
                        break;
                    case "-o":
                        if (args[i + 1] is null)
                        {
                            outputPath = string.Empty;
                            break;
                        }
                        customOutput = true;
                        outputPath = args[i + 1];
                        break;
                    case "-help":
                    case "-h":
                        printHelp();
                        return null;
                }
            }

            return new Args()
            {
                InputFile = new FileInfo(inputPath) ?? null,
                OutputFile = customOutput ? new FileInfo(outputPath) : null,
                HasCustomOutput = customOutput
            };
        }

        private static void printHelp()
        {
            StringBuilder sb = new();
            sb.AppendLine("Image processing application for LZakSuite");
            sb.AppendLine();
            sb.AppendLine("Usage: [input parameter:required] [input arguments:required] [output parameter:optional] [output parameter:optional]");
            sb.AppendLine();
            sb.AppendLine("No parameter: this help text.");
            sb.AppendLine("-h or -help: this help text");
            sb.AppendLine("-i [path] (required): path to source image.");
            sb.AppendLine("-o [path] (optional): path to destination GCODE file.");

            Console.WriteLine(sb.ToString());
        }
    }
}
