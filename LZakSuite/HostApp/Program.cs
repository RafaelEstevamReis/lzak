using HostApp.Engine;
using System;

namespace HostApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            Core c = new(new Configuration());
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
