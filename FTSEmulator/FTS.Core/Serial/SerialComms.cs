using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace FTS.Core
{
    public class SerialComms : ISerial
    {
        SerialPort Serial;
        Configuration Config;
        public bool IsOpen { get; set; }

        public event OnTryConnect TryConnect;
        public event OnConnectSuccessfull ConnectSuccessfull;
        public event OnConnectFailure ConnectFailure;

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
            var t = connect();
            t.Wait();
            return IsOpen = t.Result;
        }

        private async Task<bool> connect()
        {
            int currentTry = 1;

            do
            {
                try
                {
                    TryConnect?.Invoke(new SerialEventArgs(currentTry));
                    Serial.Open();
                    ConnectSuccessfull?.Invoke(new SerialEventArgs());
                    break;
                }
                catch (Exception ex)
                {
                    await Task.Delay(1000);
                    currentTry++;
                    if (currentTry == Config.SerialMaxCnnTries)
                    {
                        ConnectFailure?.Invoke(new SerialEventArgs(ex, true));
                    }
                    else
                    {
                        ConnectFailure?.Invoke(new SerialEventArgs(ex));
                    }
                }
            } while (currentTry < Config.SerialMaxCnnTries);

            if (Serial.IsOpen) return true;
            else return false;
        }

        // Blink por enquanto
        public void Move(byte Command)
        {

        }
    }
}
