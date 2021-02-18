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

        public float StepSizeMM { get; set; }

        private Configuration()
        {
            SerialBaudRate = 9600;
            SerialCOMPort = "COM4";
            SerialMaxCnnTries = 10;
            SerialParity = Parity.None;
            SerialDataBits = 8;
            SerialStopBits = StopBits.One;

            StepSizeMM = 2;
            WorkspaceMM = new PointF()
            {
                X = 100,
                Y = 100
            };
        }
    }
}
