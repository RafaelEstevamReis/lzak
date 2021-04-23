using System;
using System.Threading;

namespace ImageProcessorApp.Helper
{
    public class GenericHelper
    {
        public static void PrettyExit(int timeout)
        {
            if (timeout > 10) timeout = 10;
            Console.WriteLine();
            var currY = Console.CursorTop;

            for (int i = timeout; i > 0; i--)
            {
                Console.SetCursorPosition(0, currY);
                Console.Write($"Exit in {i - 1} seconds... ");
                Thread.Sleep(1000);
            }
            Console.WriteLine("Bye!");
        }
    }
}
