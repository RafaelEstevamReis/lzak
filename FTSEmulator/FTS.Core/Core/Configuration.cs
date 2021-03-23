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
            ConsoleTableDimensions = new PointI(60, 25);

            UserInputArea = new PointI(ConsoleTableDimensions.X + 2, 10);
            BottomStatuses = new PointI(2, 26);

            // serial
            SerialBaudRate = 9600;
            SerialCOMPort = "COM7";
            SerialMaxCnnTries = 10;
            SerialParity = Parity.None;
            SerialDataBits = 8;
            SerialStopBits = StopBits.One;

            StepsPerMillimiter_X = 100;
            StepsPerMillimiter_Y = 100;
            StepsPerMillimiter_Z = 10;

            WorkspaceMM = new PointF()
            {
                X = 100,
                Y = 100
            };
        }
    }
}
