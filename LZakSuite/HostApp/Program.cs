using HostApp.HostCore;
using System;

namespace HostApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            Core c = new(new());
            c.Run();
        }
    }
}
