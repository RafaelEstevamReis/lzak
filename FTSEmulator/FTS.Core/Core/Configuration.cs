using System.IO.Ports;

namespace FTS.Core
{
    public class Configuration
    {
        public static Configuration Instance { get; } = new Configuration();

        // Serial configs
        public int SerialBaudRate { get; set; }
        public string SerialCOMPort { get; set; }
        public int SerialMaxCnnTries { get; set; }
        public Parity SerialParity{ get; set; }
        public int SerialDataBits{ get; set; }
        public StopBits SerialStopBits { get; set; }

        public PointF WorkspaceMM { get; }
        // [0,0] is at Lower Left Corner!

        public int StepsPerMillimiter_X { get; set; }
        public int StepsPerMillimiter_Y { get; set; }
        public int StepsPerMillimiter_Z { get; set; }

        // DrawConfig
        public PointI UserInputArea { get; set; }
        public PointI ConsoleTableDimensions{ get; set; }
        public PointI BottomStatuses { get; set; }

        private Configuration()
        {
            // draw
            UserInputArea = new PointI(64, 10);
            ConsoleTableDimensions = new PointI(62, 25);
            BottomStatuses = new PointI(8, 26);

            // serial
            SerialBaudRate = 9600;
            SerialCOMPort = "COM4";
            SerialMaxCnnTries = 10;
            SerialParity = Parity.None;
            SerialDataBits = 8;
            SerialStopBits = StopBits.One;

            StepsPerMillimiter_X = 1;
            StepsPerMillimiter_Y = 1;
            StepsPerMillimiter_Z = 1;

            WorkspaceMM = new PointF()
            {
                X = 100,
                Y = 100
            };
        }
    }
}
