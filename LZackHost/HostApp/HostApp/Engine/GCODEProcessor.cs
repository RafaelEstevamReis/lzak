using System;
using System.IO;
using System.Text;

namespace HostApp
{
    public static class GCODEProcessor
    {
        public static string ToGCODE(this bool[,] map)
        {
            var width = map.GetLength(0);
            var lenght = map.GetLength(1);

            StringBuilder sb = new StringBuilder();

            for (int x = 0; x < width ; x++)
            {
                for (int y = 0; y < lenght; y++)
                {
                    sb.Append(map[x, y] ? "x" : " ");
                }
                sb.Append(Environment.NewLine);
            }

            string txt = sb.ToString();

            File.WriteAllText("out.txt", txt);

            return "";
        }
    }
}
