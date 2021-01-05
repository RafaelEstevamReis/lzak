using System;
using System.Drawing;

namespace Terminal
{
    public enum Corner
    {
        TopLeft, TopRight,
        LowerLeft, LowerRight
    }

    public static class Helpers
    {
        public static void DrawHorizontalLine(int length, int left = 0, int top = 0)
        {
            char character = '─';

            DrawHorizontalLine(length, new Point(left, top));
        }
        public static void DrawHorizontalLine(int length,  Point Position = default)
        {
            if (Position == default) Position = new Point(0, 0);

            char character = '─';

            Console.SetCursorPosition(Position.X, Position.Y);
            Console.Write(new string(character, length));
        }

        public static void DrawCorner(Corner corner, int left, int top)
        {
            DrawCorner(corner, new Point(left, top));
        }
        public static void DrawCorner(Corner corner, Point Position)
        {
            Console.SetCursorPosition(Position.X, Position.Y);

            if (corner == Corner.TopLeft) Console.Write("┌");
            if (corner == Corner.TopRight) Console.Write("┐");
            if (corner == Corner.LowerLeft) Console.Write("└");
            if (corner == Corner.LowerRight) Console.Write("┘");
        }
    }
}
