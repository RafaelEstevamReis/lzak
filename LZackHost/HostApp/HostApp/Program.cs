using System;

namespace HostApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            Core c = Core.GetInstance();
            ImageProcessor.ReportPercentage += ImageProcessor_ReportPercentage;
            c.Run();
        }

        private static void ImageProcessor_ReportPercentage(object sender, PercentageEventArgs e)
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(Console.CursorTop, 0);
            Console.Write(new string(' ', 15));
            Console.SetCursorPosition(Console.CursorTop, 0);
            Console.Write($"{e.Progress}/100");
        }
    }
}
