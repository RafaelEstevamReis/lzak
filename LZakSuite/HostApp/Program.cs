using HostApp.HostCore;
using System;

namespace HostApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            HostEngine c = new(new HostConfig());
            c.Progress += ImageProcessor_ReportPercentage;
            c.Run();
        }

        private static void ImageProcessor_ReportPercentage(object sender, PercentageEventArgs e)
        {
            Console.SetCursorPosition(Console.CursorTop, 0);
            Console.Write($"{e.Percentage}                   ");
        }
    }
}
