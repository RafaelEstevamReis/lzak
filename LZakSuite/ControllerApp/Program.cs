using ControllerApp.ControllerCore;
using System;

namespace ControllerApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            var engine = new ControllerEngine(new ControllerConfig());
            engine.Run();
        }
    }
}
