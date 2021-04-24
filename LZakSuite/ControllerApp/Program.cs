using ControllerApp.ControllerCore;
using System;
using System.IO;

namespace ControllerApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            FileInfo GCODEFile = null;//new FileInfo(args[0]);

            var engine = new Engine(new Config(), GCODEFile);
            engine.Run();
        }
    }
}
