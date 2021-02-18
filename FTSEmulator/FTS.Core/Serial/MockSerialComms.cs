namespace FTS.Core
{
    public class MockSerialComms : ISerial
    {
        public event OnTryConnect TryConnect;
        public event OnConnectSuccessfull ConnectSuccessfull;
        public event OnConnectFailure ConnectFailure;

        public bool IsOpen { get; set; }

        // mock serial class, for testing
        public void Move(byte Command) { }
        public bool Open() => true;
    }
}
