namespace HostApp.Interfaces
{


    public interface IConfig 
    {
        public bool AllowBitmap { get; }
        public bool AllowPNG { get; }
        public bool AllowJPEG { get; }
        public string ImagePath { get; }
        public OperationMode Mode { get; }
        public string LON_Command { get; }
        public string LOFF_Command { get;  }
        public double ZON_HeigthMM { get;  }
        public double ZOFF_HeigthMM { get; }
        public int PointsPerMM { get; set; }
    }
}
