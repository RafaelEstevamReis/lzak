using HostApp.Interfaces;
using System;

namespace HostApp.HostCore
{
    public class HostConfig : IHostConfig
    {
        // startup/image load config
        public bool AllowBitmap { get; }
        public bool AllowPNG { get; }
        public bool AllowJPEG { get; }
        public string ImagePath { get; set; }

        // GCODEProcessor configs
        public OperationMode Mode { get; set; } = OperationMode.LaserMode;

        public string LON_Command { get; set; } = "M3";
        public string LOFF_Command { get; set; } = "M5";

        public double ZON_HeigthMM { get; set; } = 0;
        public double ZOFF_HeigthMM { get; set; } = 1.2;

        public int pointsPerMM;
        public int PointsPerMM
        {
            get => pointsPerMM;
            set
            {
                if (value == 0) throw new DivideByZeroException("PointsPerMM cannot be zero.");
                if (value < 0) throw new ArgumentOutOfRangeException("PointsPerMM cannot be less than zero.");
                pointsPerMM = value;
            }
        }
        public HostConfig()
        {
            AllowBitmap = true;
            AllowPNG = true;
            AllowJPEG = true;

            Mode = OperationMode.LaserMode;
            LON_Command = "M3";
            LOFF_Command = "M5";
            ZON_HeigthMM = 0;
            ZOFF_HeigthMM = 1.2;

            pointsPerMM = 1;

            ImagePath = "pic2.jpg";
        }
    }
}
