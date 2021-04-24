using ControllerApp.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerApp.ControllerCore
{
    public class Engine
    {
        private Memory Memory;

        public Engine( Config Config, FileInfo GCODEFile = null)
        {
            if (Config is null) throw new ArgumentException("Invalid configuration.");

            Memory = new(Config);
            Memory.GCODEFile = GCODEFile;
            Console.CursorVisible = false;
        }
        
        public void Run()
        {
            SerialConnect();

            while (!Memory.ShutdownToken.IsCancellationRequested)
            {
                // SerialListen();

                // TODO future: manual commands will be here
                // manual commands should generate GCODE to move

                var command = string.Empty;

                if(Memory.GCODEFile is null)
                {
                    Console.Write("Type in GCODE command: ");
                    command = Console.ReadLine();
                }

                if (string.IsNullOrEmpty(command)) continue;

                processCommand(command);
            }
        }
        private void SerialConnect()
        {
            Memory.Serial.Open();
        }
        private void SerialListen()
        {
            byte[] buffer = new byte[256];

            if (Memory.Serial.IsOpen)
            {
                try
                {
                    if (Memory.Serial.Read(buffer, 0, buffer.Length) > 0)
                    {
                        var message = Encoding.UTF8.GetString(buffer);
                        message = message.Replace("\0", "");

                        var split = message.Split("\r\n");

                        foreach (var item in split) { } // TODO make this method output to message sink
                                                        // memory.LogMessageQueue.Enqueue(message);
                    }
                }
                catch { }
            }
        }

        private void processCommand(string command)
        {
            // TODO Process command
            if (command.StartsWith("G")) processGCODE(command);
            if (command.StartsWith("M")) processMCODE(command);
        }
        private void processMCODE(string command)
        {
            command = command.Trim();

            switch (command)
            {
                case "M3":
                    Memory.SpindleToggle = true;
                    break;
                case "M5":
                    Memory.SpindleToggle = false;
                    break;
            }
        }
        private void processGCODE(string command)
        {
            command = command
                      .Trim();

            var parts = command
                        .Split(" ");

            if(parts.Length != 3)
            {
                Console.WriteLine("Error in command");
                return;
            }

            bool commands_ok = verifyCommands(parts);

            float result_X = 0F;
            float result_Y = 0F;
            if(commands_ok)
            {
                bool x_ok = MathHelper.TryGetCoordinates(parts, Axis.X, out result_X);
                bool y_ok = MathHelper.TryGetCoordinates(parts, Axis.Y, out result_Y);

                if(x_ok && y_ok)
                {

                }
            }
        }
        
        private bool verifyCommands(string[] parts)
        {
            if (parts is null) return false;
            if (parts.Length != 3) return false;
            if (parts[0] != "G1") return false;
            if (!parts[1].StartsWith("X")) return false;
            if (!parts[2].StartsWith("Y")) return false;

            return true;
        }
    }
}
