using FTS.Core;

namespace FTS.UI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            StateMachine.Run(new MockSerialComms());
        }
    }
}

