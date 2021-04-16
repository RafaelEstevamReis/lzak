using ControllerApp.Enums;
using ControllerApp.MathResources;
using System;
using System.IO.Ports;

namespace ControllerApp.ControllerCore
{
    public class Config 
    {
        public int SerialBaudRate { get; set; }
        public string SerialCOMPort { get; set; }
        public Parity SerialParity { get; set; }
        public int SerialDataBits { get; set; }
        public StopBits SerialStopBits { get; set; }
        public bool EnableDtr { get; set; }
        public bool EnableRts { get; set; }
        public PointF WorkspaceMM { get; }
        public int StepsPerMillimiter_X { get; set; }
        public int StepsPerMillimiter_Y { get; set; }
        public int StepsPerMillimiter_Z { get; set; }
        public MoveSpeed DefaultMotorSpeed { get; set; }
        public int FastMotorSpeedMs   { get; set; }
        public int MediumMotorSpeedMs { get; set; }
        public int SlowMotorSpeedMs { get; set; }

        public Config()
        {
            SerialBaudRate = 9600;
            SerialCOMPort = "COM7";
            SerialParity = Parity.None;
            SerialDataBits = 8;
            SerialStopBits = StopBits.One;
            EnableDtr = true;
            EnableRts = true;
            StepsPerMillimiter_X = 20;
            StepsPerMillimiter_Y = 20;
            StepsPerMillimiter_Z = 1;
            WorkspaceMM = new PointF(100, 100);
        }
    }
}
