using ControllerApp.MathResources;
using System;
using System.Runtime.InteropServices;

namespace ControllerApp.Helpers
{
    public class ConsoleHelper
    {
        private object LockObj;

        Point LogAreaPosition = new Point(60, 15);

        public ConsoleHelper(object LockObj)
        {
            if (LockObj is null) throw new Exception("null or invalid lock object");
            this.LockObj = LockObj;
        }

        public void Write(string text, 
                          Point location = null, 
                          int cleanBeforeWrite = 0, 
                          bool showCursor = false, 
                          bool jumpLine = false, bool writeOnLogArea = false)
        {
            if (location is null) location = new Point();

            if (cleanBeforeWrite < 0) throw new ArgumentException("cleanBeforeWrite cannot be < 0");
            if (location.X < 0) throw new ArgumentException("Invalid location to write {x < 0}");
            if (location.Y < 0) throw new ArgumentException("Invalid location to write {y < 0}");

            lock (LockObj)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Console.CursorVisible = showCursor;
                }

                if (cleanBeforeWrite > 0 && text.Length < cleanBeforeWrite)
                {
                    Console.SetCursorPosition(location.X, location.Y);
                    Console.Write(new string(' ', cleanBeforeWrite));
                }

                if (!writeOnLogArea) Console.SetCursorPosition(location.X, location.Y);
                else
                {
                    Console.SetCursorPosition(LogAreaPosition.X, LogAreaPosition.Y);
                    Console.WriteLine(new string((char)32, Console.WindowWidth - LogAreaPosition.X));
                    Console.SetCursorPosition(LogAreaPosition.X, LogAreaPosition.Y);
                }

                if (jumpLine) Console.WriteLine(text);
                else Console.Write(text);
            }
        }
    }
}
