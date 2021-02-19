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

        public SerialPort Serial { get; } = new SerialPort("MOCK_COM", 9600, Parity.None, 8, StopBits.One);

        // mock serial class, for testing
        public void Move(Step x, Step y, Step z) { }
        public bool Open() => true;
        public void ListenAsync() 
        {
            TryConnect?.Invoke(new SerialConnectEventArgs());
            ConnectSuccessful?.Invoke(new SerialConnectEventArgs());
        }
    }
}
