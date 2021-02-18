using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FTS.Core
{
    public class SerialComms : ISerial
    {
        public SerialPort Serial { get; }
        Configuration Config;
        public bool IsOpen { get; set; }

        public event OnTryConnect TryConnect;
        public event OnConnectSuccessful ConnectSuccessful;
        public event OnConnectFailure ConnectFailure;

        public event OnEngravingToggle EngravingToggle;
        
        public SerialComms()
        {
            Config = Configuration.Instance;

            if (Config is null) throw new ArgumentException("config cannot be null.");

            Serial = new SerialPort(Config.SerialCOMPort,
                                    Config.SerialBaudRate,
                                    Config.SerialParity,
                                    Config.SerialDataBits,
                                    Config.SerialStopBits);
        }
        public bool Open()
        {
            // Conectar ao arduino.
            // tem um retorno em bool e um eventos dizendo o status.
            return IsOpen = connect();
        }
        private bool connect()
        {
            int currentTry = 1;

            do
            {
                try
                {
                    TryConnect?.Invoke(new SerialConnectEventArgs(currentTry));
                    Serial.Open();
                    ConnectSuccessful?.Invoke(new SerialConnectEventArgs());
                    break;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(1000);
                    currentTry++;
                    if (currentTry == Config.SerialMaxCnnTries)
                    {
                        ConnectFailure?.Invoke(new SerialConnectEventArgs(ex, true));
                    }
                    else
                    {
                        ConnectFailure?.Invoke(new SerialConnectEventArgs(ex));
                    }
                }
            } while (currentTry < Config.SerialMaxCnnTries);

            if (Serial.IsOpen) return true;
            else return false;
        }
        // Blink por enquanto
        public void Move(Step step)
        {
            using var bw = new BinaryWriter(Serial.BaseStream);

            // if(step == Step.StepLeft) moveStepperMotor(bw, 

            // concept blinking art
            //while (true)
            //{
            //    for (int i = 0; ; i++)
            //    {
            //        sw.Write((byte)(1 << (i % 6)));
            //        Thread.Sleep(50);
            //    }
            //}
        }

        private static void moveStepperMotor(BinaryWriter bw, params Step[] Steps)
        {
            if (Steps.Length != 3) throw new ArgumentException("Not enough steps provided.");

            byte bX = (byte)Steps[0];
            byte bY = (byte)Steps[1];
            byte bZ = (byte)Steps[2];

            var b = (bX << 4) +
                    (bY << 2) +
                     bZ;

            bw.Write((byte)b);
        }

        public async void ListenAsync()
        {
            using var sr = new BinaryReader(Serial.BaseStream, encoding: Encoding.ASCII);

            int len;
            var buffer = new byte[512];

            while (true)
            {
                await Task.Delay(10);
                if ((len = sr.Read(buffer, 0, buffer.Length)) == 0) continue;

                var dados = Encoding.ASCII.GetString(buffer, 0, len);

                if (dados.Contains("STOP"))
                {
                    EngravingToggle?.Invoke(new SerialCallBackEventArgs(true));
                }
                else
                {
                    EngravingToggle?.Invoke(new SerialCallBackEventArgs(false));
                }
            }
        }
    }
}
