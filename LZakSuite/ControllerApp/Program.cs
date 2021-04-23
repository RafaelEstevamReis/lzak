using ControllerApp.ControllerCore;
using System;

namespace ControllerApp
{
    class Program
    {
        public static void Main(string[] args)
        {

            var engine = new Engine(new Config());
            engine.Run();
        }
    }
}
