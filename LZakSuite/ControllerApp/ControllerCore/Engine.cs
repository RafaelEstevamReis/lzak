using ControllerApp.Enums;
using ControllerApp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ControllerApp.ControllerCore
{
    public class Engine
    {
        private Memory Memory;

        public Engine(Config Config, FileInfo GCODEFile = null)
        {
            if (Config is null) throw new ArgumentException("Invalid configuration.");

            Memory = new(Config);
            Memory.GCODEFile = GCODEFile;
            Console.CursorVisible = false;
        }

        public void Run()
        {
            Console.WriteLine($"Connecting to serial port at {Memory.Config.SerialCOMPort}...");
            SerialConnect();

            if (!Memory.Serial.IsOpen)
            { 
                Console.WriteLine($"Unable to connect to {Memory.Config.SerialCOMPort}!. Check your connections.");
                return;
            }

            Console.WriteLine($"Connected to port {Memory.Config.SerialCOMPort}!");

            bool fromFile = false;
            string[] commands = null;

            if(Memory.GCODEFile is not null)
            {
                Console.WriteLine($"Running on GCODE file input mode...");
                fromFile = true;
                commands = getCommandsFromFile(Memory.GCODEFile).ToArray();
            }

            if(!fromFile) Console.WriteLine($"GCODE file provided at {Memory.GCODEFile.FullName}...");

            while (!Memory.ShutdownToken.IsCancellationRequested)
            {
                // SerialListen();

                // TODO future: manual commands will be here
                // manual commands should generate GCODE to move

                if (!fromFile)
                {
                    Console.Write("Type in GCODE command: ");
                    var command = Console.ReadLine();
                    if(string.IsNullOrEmpty(command)) continue;
                    processCommand(command);
                    continue;
                }

                Console.WriteLine("Processing commands...");
                foreach(var cmd in commands)
                {
                    Console.WriteLine(cmd);
                    processCommand(cmd);
                }

                Console.WriteLine("Done! Press a key to quit.");
                Console.ReadKey();
                break;
            }
        }

        private IEnumerable<string> getCommandsFromFile(FileInfo gCODEFile)
        {
            if (!gCODEFile.Exists) yield return default;

            var file = File.ReadAllLines(gCODEFile.FullName);

            foreach(var l in file)
            {
                yield return l;
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
            command = command.Trim();
            var parts = command.Split(" ");
            bool commands_ok = verifyCommands(parts);

            if (commands_ok)
            {
                bool x_ok = MathHelper.TryGetCoordinates(parts, Axis.X, out float result_X);
                bool y_ok = MathHelper.TryGetCoordinates(parts, Axis.Y, out float result_Y);
                bool z_ok = MathHelper.TryGetCoordinates(parts, Axis.Y, out float result_Z);

                if (!x_ok || !y_ok || !z_ok)
                {
                    Console.WriteLine("Error in command. Type in again.");
                    return;
                }

                Memory.Motor.MoveTo(result_X, result_Y);
            }
        }

        private bool verifyCommands(string[] parts)
        {
            if (parts is null) return false;
            if (parts.Length < 3 || parts.Length > 5) return false;
            if (parts[0] != "G1") return false;
            if (!parts[1].StartsWith("X")) return false;
            if (!parts[2].StartsWith("Y")) return false;
            if (parts.Length > 3 && !parts[3].StartsWith("Z")) return false;

            return true;
        }
    }
}
