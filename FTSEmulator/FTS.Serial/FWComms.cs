using FTS.Core;
using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace FTS.Serial
{
    public class SerialEventArgs
    {
        public int CurrentTry { get; set; }
        public bool LastTry { get; set; }

        public SerialEventArgs(bool LastTry = false) => this.LastTry = LastTry;
        public SerialEventArgs(int CurrentTry) => this.CurrentTry = CurrentTry;
    }

    public class FWComms
    {
        public delegate void OnTryConnect(SerialEventArgs e);
        public event OnTryConnect TryConnect;

        public delegate void OnConnectSuccessfull(SerialEventArgs e);
        public event OnConnectSuccessfull ConnectSuccessfull;

        public delegate void OnConnectFailure(SerialEventArgs e);
        public event OnConnectFailure ConnectFailure;

        SerialPort Serial;
        Configuration Config;

        public FWComms()
        {
            Config = Configuration.Instance;

            if (Config is null) throw new ArgumentException("config cannot be null.");
            Serial = new SerialPort(Config.SerialCOMPort,
                                    Config.SerialBaudRate,
                                    Parity.None,
                                    0,
                                    StopBits.One);

            // Conectar ao arduino.
            // tem um retorno em bool e um evento dizendo o status; 
            // deixei status pro evento e não to usando o bool pra nada.
            var t = connect();
            t.Wait();
            bool resultado = t.Result;
        }

        // Blink por enquanto
        public void Move(byte Command)
        {
            
        }

        private async Task<bool> connect()
        {
            int currentTry = 0;

            do
            {
                try
                {
                    TryConnect?.Invoke(new SerialEventArgs(currentTry));
                    Serial.Open();
                    ConnectSuccessfull?.Invoke(new SerialEventArgs());
                }
                catch
                {
                    await Task.Delay(1000);
                    currentTry++;
                    if (currentTry == Config.MaxSerialCnnTries)
                    {
                        ConnectFailure?.Invoke(new SerialEventArgs(true));
                    }
                    else
                    {
                        ConnectFailure?.Invoke(new SerialEventArgs());
                    }
                }
            } while (currentTry < Config.MaxSerialCnnTries);

            if (Serial.IsOpen) return true;
            else return false;
        }
    }
}
