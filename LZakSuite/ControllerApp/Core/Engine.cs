using ControllerApp.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ControllerApp.Core
{
    public class Engine
    {
        private Memory memory;

        public Engine(IConfiguration Config)
        {
            if (Config is null) throw new ArgumentException("Invalid configuration.");
            memory = new(Config);
            Console.CursorVisible = false;
        }

        /// <summary>
        /// The main entry point for the controller suite. It can operate manually or via GCODE-based instructions.
        /// </summary>
        /// <param name="GCODE">The text containing a valid GCODE.</param>
        public void Run(string GCODE = null)
        {
            // if no gcode is provided, manual operation is automatically activated
            if (GCODE is null) memory.ManualOperation = true;

            // keep checking for serial communication
            Task.Run(() => RuntimeResources.SerialKeepAlive(memory));

            // keep listening for serial feedback, if available or possible
            Task.Run(() => RuntimeResources.SerialListen(memory));

            // run 
            while (!memory.ShutdownToken.IsCancellationRequested)
            {
                if (!memory.Serial.IsOpen) continue;

                if (memory.ManualOperation)
                {
                    RuntimeResources.ManualOperation(memory);
                    Thread.Sleep(100);
                    continue;
                }

                RuntimeResources.ProcessGCODE(memory);
            }
        }
    }
}
