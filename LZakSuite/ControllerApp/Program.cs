using ControllerApp.ControllerCore;
using System;
using System.IO;
using System.Text;

namespace ControllerApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var GCODEFile = processArgs(args, out bool exit);

                if (exit) return;

                if (GCODEFile is null) Console.WriteLine("Failed to load GCODE file. Will start on user input mode.");

                var engine = new Engine(new Config(), GCODEFile);

                engine.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Will exit.");
            }
        }

        private static FileInfo processArgs(string[] args, out bool exit)
        {
            string inputPath = string.Empty;

            try
            {
                if (args is null) throw new ArgumentException("No file path provided.");

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
                        case "--help":
                        case "-h":
                            printHelp();
                            exit = true;
                            return null;
                    }
                }
            }
            catch (Exception)
            {
                exit = true;
                return null;
            }

            exit = false;
            return new FileInfo(inputPath) ?? null;
        }

        private static void printHelp()
        {
            StringBuilder sb = new();
            sb.AppendLine("Controller application for LZakSuite");
            sb.AppendLine();
            sb.AppendLine("Usage: [parameter:optional] [arguments:optional]");
            sb.AppendLine();
            sb.AppendLine("No parameter: manual command input mode.");
            sb.AppendLine("-h or -help: this help text");
            sb.AppendLine("-i [path]: path to GCODE file.");

            Console.WriteLine(sb.ToString());
        }
    }
}
