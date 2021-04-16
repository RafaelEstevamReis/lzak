using System;

namespace HostApp.HostCore
{
    public class Config
    {
        public bool AllowBitmap { get; }
        public bool AllowPNG { get; }
        public bool AllowJPEG { get; }
        public string ImagePath { get; set; }
        public OperationMode Mode { get; set; } 
        public string LON_Command { get; set; }
        public string LOFF_Command { get; set; } 
        public double ZON_HeigthMM { get; set; } 
        public double ZOFF_HeigthMM { get; set; } 
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
        public Config()
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
