using ControllerApp.MathResources;
using System;
using System.IO.Ports;

namespace ControllerApp.Interfaces
{
    public interface IConfiguration
    {
        public int SerialBaudRate { get; set; }
        public string SerialCOMPort { get; set; }
        public int SerialMaxCnnTries { get; set; }
        public Parity SerialParity { get; set; }
        public int SerialDataBits { get; set; }
        public StopBits SerialStopBits { get; set; }
        public int MaxSerialReconnectTries { get; set; }
        public bool EnableDtr { get; set; }
        public bool EnableRts { get; set; }

        public PointF WorkspaceMM { get; }
        public int StepsPerMillimiter_X { get; set; }
        public int StepsPerMillimiter_Y { get; set; }
        public int StepsPerMillimiter_Z { get; set; }


        public ConsoleKey NoOperation { get; set; }
        public ConsoleKey End { get; set; }
        public ConsoleKey Up { get; set; }
        public ConsoleKey Down { get; set; }
        public ConsoleKey Left { get; set; }
        public ConsoleKey Right { get; set; }
        public ConsoleKey ToolToggle { get; set; }
        public ConsoleKey Halt { get; set; }
        public ConsoleKey Reset { get; set; }
        public ConsoleKey Home { get; set; }
        public ConsoleKey EnableManualOperation { get; set; }
        public ConsoleKey DisableManualOperation { get; set; }
    }
}
