using System;

namespace FTS.Core
{
    public class ConsoleWriter
    {
        object lockObj;
        public ConsoleWriter(object lockObj)
        {
            if (lockObj is null) throw new Exception("Invalid lock object");
            this.lockObj = lockObj;
        }

        public void HorizontalLine(PointI position, int len)
        {
            lock (lockObj)
            {
                try
                {
                    Console.SetCursorPosition(position.X, position.Y);
                    Console.Write(new string('-', len));
                }
                catch { return; }
            }
        }

        public void VerticalLine(PointI position, int height)
        {
            lock (lockObj)
            {
                try
                {
                    for (int i = 0; i < height; i++)
                    {
                        Console.SetCursorPosition(position.X, position.Y++);
                        Console.Write('|');
                    }
                }
                catch { return; }
            }
        }

        // Console Writer, centralized, so onscreen texts aren't going to get messy.
        public void WriteOnConsole(string message, PointI location, int cleanLen, bool showCursor = false)
        {
            lock (lockObj)
            {
                Console.CursorVisible = showCursor;

                Console.SetCursorPosition(location.X, location.Y);
                Console.Write(new string(' ', cleanLen));
                Console.SetCursorPosition(location.X, location.Y);
                Console.Write(message);

                if (Console.CursorVisible) Console.CursorVisible = false;
            }
        }
        public void CleanLine(PointI pos, int cleanLen)
        {
            WriteOnConsole(string.Empty, pos, cleanLen);
            Console.SetCursorPosition(pos.X, pos.Y);
        }
    }
}
