using System;
using System.IO.Ports;

namespace FTS.Core
{
    public delegate void OnTryConnect(SerialConnectEventArgs e);
    public delegate void OnConnectSuccessful(SerialConnectEventArgs e);
    public delegate void OnConnectFailure(SerialConnectEventArgs e);

    public delegate void OnEngravingToggle(SerialCallBackEventArgs e);

    public class SerialConnectEventArgs
    {
        public int CurrentTry { get; set; }
        public bool LastTry { get; set; }
        public Exception Exception { get; set; }

        public SerialConnectEventArgs(bool LastTry = false) => this.LastTry = LastTry;
        public SerialConnectEventArgs(int CurrentTry) => this.CurrentTry = CurrentTry;
        public SerialConnectEventArgs(Exception Exception) => this.Exception = Exception;
        public SerialConnectEventArgs(Exception Exception, bool LastTry = false)
        {
            this.Exception = Exception;
            this.LastTry = LastTry;
        }
    }

    public class SerialCallBackEventArgs
    {
        public bool ENDSTOPActivated { get; set; }
        public SerialCallBackEventArgs(bool ENDSTOPActivated) => this.ENDSTOPActivated = ENDSTOPActivated;
    }

    public interface ISerial
    {
        public event OnTryConnect TryConnect;
        public event OnConnectSuccessful ConnectSuccessful;
        public event OnConnectFailure ConnectFailure;

        public event OnEngravingToggle EngravingToggle;

        public SerialPort Serial { get; }

        public bool IsOpen { get; }

        public IDriver Driver { get; }

        public bool Open();
        public void Move(Step x, Step y, Step z);
        void ListenAsync();
    }
}
