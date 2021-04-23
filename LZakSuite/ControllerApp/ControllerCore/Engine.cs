using System;
using System.Text;

namespace ControllerApp.ControllerCore
{
    public class Engine
    {
        private Memory Memory;

        public Engine(Config Config)
        {
            if (Config is null) throw new ArgumentException("Invalid configuration.");
            Memory = new(Config);
            Console.CursorVisible = false;
        }
        
        public void Run()
        {
            SerialConnect();

            while (!Memory.ShutdownToken.IsCancellationRequested)
            {
                SerialListen();
                
                // TODO future: manual commands will be here
                // manual commands should generate GCODE to move

                var command = Console.ReadLine();
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

            throw new NotImplementedException();
        }
        private void processMCODE(string command)
        {
            throw new NotImplementedException();
        }
        private void processGCODE(string command)
        {
            throw new NotImplementedException();
        }
    }
}
