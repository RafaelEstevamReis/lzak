using ControllerApp.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ControllerApp.Core
{
    public class Engine
    {
        private Memory Memory;

        public Engine(IConfiguration Config)
        {
            if (Config is null) throw new ArgumentException("Invalid configuration.");
            Memory = new(Config);
            Console.CursorVisible = false;
        }

        /// <summary>
        /// The main entry point for the controller suite. It can operate manually or via GCODE-based instructions.
        /// </summary>
        /// <param name="GCODE">The text containing a valid GCODE.</param>
        public void Run(string GCODE = null)
        {
            // if no gcode is provided, manual operation is automatically activated
            if (GCODE is null) Memory.ManualOperation = true;

            // keep checking for serial communication
            Task.Run(() => RuntimeResources.SerialKeepAlive(Memory));

            // keep listening for serial feedback, if available or possible
            Task.Run(() => RuntimeResources.SerialListen(Memory));

            Task.Run(() => RuntimeResources.MessageEventsListen(Memory));

            // run 
            while (!Memory.ShutdownToken.IsCancellationRequested)
            {
                if (!Memory.Serial.IsOpen) continue;

                if (Memory.ManualOperation)
                {
                    RuntimeResources.ManualOperation(Memory);
                    Thread.Sleep(100);
                    continue;
                }

                RuntimeResources.ProcessGCODE(Memory);
            }
        }
    }
}
