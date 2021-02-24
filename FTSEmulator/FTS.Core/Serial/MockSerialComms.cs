using System;
using System.IO.Ports;

namespace FTS.Core
{
    public class MockSerialComms : ISerial
    {
        public event OnTryConnect TryConnect;
        public event OnConnectSuccessful ConnectSuccessful;
        public event OnConnectFailure ConnectFailure;
        public event OnEngravingToggle EngravingToggle;

        public bool IsOpen { get; set; }
        bool SimulateSerialError = true;

        public SerialPort Serial { get; } = new SerialPort("MOCK_COM", 9600, Parity.None, 8, StopBits.One);

        // mock serial class, for testing
        public void Move(Step x, Step y, Step z) { }
        public bool Open() => !SimulateSerialError;
        public void ListenAsync() 
        {
            TryConnect?.Invoke(new SerialConnectEventArgs());
            if (!SimulateSerialError) ConnectSuccessful?.Invoke(new SerialConnectEventArgs());
            else ConnectFailure?.Invoke(new SerialConnectEventArgs(new Exception("ERR::MOCKCONN_ERROR")));
        }
    }
}
