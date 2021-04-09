using ControllerApp.Core;
using System.IO;
using System.IO.Ports;

namespace ControllerApp
{
    class Program
    {
        public static void Main(string[] args)
        {

            //SerialPort cmm = new SerialPort("COM7", 9600);
            //cmm.DtrEnable = true;
            //cmm.Open();

            //using (var ts = new StreamReader(cmm.BaseStream))
            //{
            //    while (true) System.Console.WriteLine(ts.ReadLine());
            //}

            var engine = new Engine(new Configuration());
            engine.Run();
        }
    }
}
