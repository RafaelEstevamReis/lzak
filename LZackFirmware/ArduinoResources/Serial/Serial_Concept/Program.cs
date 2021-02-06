using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Serial
{
    class Program
    {
        public enum Step
        {
            StepLeft = 1,
            StepRight = 2,
            None = 0,
        }

        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            SerialPort sp = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            Console.WriteLine($"Abrindo {sp.PortName}..");
            while (true)
            {
                try
                {
                    sp.Open();
                    break;
                }
                catch { }
            }
            Console.WriteLine("Look at me go");

            Task.WaitAll(
              doRead(sp.BaseStream),
              doWrite(sp.BaseStream));
        }

        private static async Task doRead(Stream stream)
        {
            using var sr = new BinaryReader(stream, encoding: Encoding.ASCII);

            int len;
            var buffer = new byte[512];

            while (true)
            {
                await Task.Delay(10);
                if ((len = sr.Read(buffer, 0, buffer.Length)) == 0) continue;

                var dados = Encoding.ASCII.GetString(buffer, 0, len);
                Console.Write(dados);
            }
        }
        private static async Task doWrite(Stream stream)
        {
            using var sw = new BinaryWriter(stream);

            while (true)
            {
                await Task.Delay(10);

                if (!Console.KeyAvailable) continue;

                var k = Console.ReadKey(true);

                if (k.Key == ConsoleKey.Q) sw.Write((byte)0);
                if (k.Key == ConsoleKey.A) sw.Write((byte)0x01);
                if (k.Key == ConsoleKey.S) sw.Write((byte)0b00000010);
                if (k.Key == ConsoleKey.D) sw.Write((byte)0b00000100);
                if (k.Key == ConsoleKey.F) sw.Write((byte)0b00001000);
                if (k.Key == ConsoleKey.G) sw.Write((byte)0b00010000);
                if (k.Key == ConsoleKey.H) sw.Write((byte)0b00100000);
                if (k.Key == ConsoleKey.W) sw.Write((byte)0xFF);

                if (k.Key == ConsoleKey.E) sw.Write((byte)0b01010101);
                if (k.Key == ConsoleKey.R) sw.Write((byte)0b00101010);

                //if (k.Key == ConsoleKey.RightArrow) stepMotor(sw, x: Step.StepLeft, y: Step.None, z: Step.StepRight);
                //if (k.Key == ConsoleKey.LeftArrow) stepMotor(sw, x: Step.None, y: Step.StepRight, z: Step.StepLeft);

                if (k.Key == ConsoleKey.F1)
                {
                    for (int i = 0; ; i++)
                    {
                        await Task.Delay(20);
                        sw.Write((byte)(1 << (i % 6)));

                        if (!Console.KeyAvailable) continue;
                        k = Console.ReadKey(true);

                        if (k.Key == ConsoleKey.Spacebar) break;

                    }
                }
                sw.Flush();
            }
        }

        private static void stepMotor(BinaryWriter sw, Step x, Step y, Step z)
        {
            byte bX = (byte)x;
            byte bY = (byte)y;
            byte bZ = (byte)z;

            var b = (bX << 4) +
                   (bY << 2) +
                   bZ;

            sw.Write((byte)b);
        }
    }
}
