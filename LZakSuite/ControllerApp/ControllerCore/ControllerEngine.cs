using ControllerApp.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ControllerApp.ControllerCore
{
    public class ControllerEngine
    {
        private Memory Memory;

        public ControllerEngine(IControllerConfig Config)
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

            Memory.GCODE = GCODE;

            // keep checking for serial communication
            Task.Run(() => RuntimeResources.SerialKeepAlive(Memory));

            // keep listening for serial feedback, if available or possible
            Task.Run(() => RuntimeResources.SerialListener(Memory));

            // run 
            while (!Memory.ShutdownToken.IsCancellationRequested)
            {
                // if no serial, wait for new tries until connect of failure.
                // No command should be issued in case of no connection.
                if (!Memory.Serial.IsOpen) continue;

                switch(Memory.ManualOperation)
                {
                    case true:
                        RuntimeResources.ManualOperation(Memory);
                        break;
                    case false:
                        RuntimeResources.ProcessGCODE(Memory);
                        break;
                }
            }

            // wait a bit before gone
            Thread.Sleep(3000);
        }
    }
}
