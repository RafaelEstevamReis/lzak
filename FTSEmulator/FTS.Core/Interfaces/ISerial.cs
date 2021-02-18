using System;

namespace FTS.Core
{
    public delegate void OnTryConnect(SerialEventArgs e);
    public delegate void OnConnectSuccessfull(SerialEventArgs e);
    public delegate void OnConnectFailure(SerialEventArgs e);

    public class SerialEventArgs
    {
        public int CurrentTry { get; set; }
        public bool LastTry { get; set; }
        public Exception Exception { get; set; }

        public SerialEventArgs(bool LastTry = false) => this.LastTry = LastTry;
        public SerialEventArgs(int CurrentTry) => this.CurrentTry = CurrentTry;
        public SerialEventArgs(Exception Exception) => this.Exception = Exception;
        public SerialEventArgs(Exception Exception, bool LastTry = false)
        {
            this.Exception = Exception;
            this.LastTry = LastTry;
        }
    }

    public interface ISerial
    {
        public event OnTryConnect TryConnect;
        public event OnConnectSuccessfull ConnectSuccessfull;
        public event OnConnectFailure ConnectFailure;

        public bool IsOpen { get; set; }

        public bool Open();
        public void Move(byte Command);
    }
}
