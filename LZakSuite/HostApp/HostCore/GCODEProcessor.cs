using HostApp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HostApp.HostCore
{
    public partial class GCODEProcessor
    {
        public IHostConfig Config;
        
        public GCODEProcessor(IHostConfig Config)
        {
            if (Config is null) throw new ArgumentNullException("No configuration provided.");
            this.Config = Config;
        }

        public string ToGCODE(bool[,] map)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var line in getGCODE(map))
            {
                string l2 = line;

                if (l2.Contains(".000")) l2 = l2.Replace(".000", "");

                sb.Append(l2);
                sb.Append("\r\n");
            }
            return sb.ToString();
        }

        private IEnumerable<string> getGCODE(bool[,] map)
        {
            var width = map.GetLength(0);
            var height = map.GetLength(1);

            bool toolStatus = false;
            double xPos = 0;
            double yPos = 0;

            for (int y = 0; y < height; y++)
            {
                // reseta o xPos
                xPos = 0;
                for (int x = 0; x < width; x++)
                {
                    // Cuidado com o fim da linha
                    if (map[x, y] == toolStatus) continue;

                    xPos = x / (double)Config.PointsPerMM;
                    // Se preciso ligar o tool...
                    if (map[x, y] == true)
                    {
                        if (Config.Mode == OperationMode.LaserMode)
                        {
                            yield return $"G1 X{xPos:0.000} Y{yPos:0.000}"; // mm
                            yield return Config.LON_Command;
                        }
                        else if (Config.Mode == OperationMode.ZMode)
                        {
                            yield return $"G1 X{xPos:0.000} Y{yPos:0.000} Z{Config.ZOFF_HeigthMM:0.000}"; // mm
                            yield return $"G1 Z{Config.ZON_HeigthMM:0.000}"; // mm
                        }
                        toolStatus = true;
                    }
                    else // Se preciso DESligar o tool...
                    {
                        if (Config.Mode == OperationMode.LaserMode)
                        {
                            yield return $"G1 X{xPos:0.000} Y{yPos:0.000}"; // mm
                            yield return Config.LOFF_Command;
                        }
                        else if (Config.Mode == OperationMode.ZMode)
                        {
                            yield return $"G1 X{xPos:0.000} Y{yPos:0.000} Z{Config.ZON_HeigthMM:0.000}"; // mm
                            yield return $"G1 Z{Config.ZOFF_HeigthMM:0.000}"; // mm
                        }
                        toolStatus = false;
                    }
                }
                // Terminou com Tool ON?
                if (toolStatus == true)
                {
                    xPos = width / (double)Config.PointsPerMM;
                    if (Config.Mode == OperationMode.LaserMode)
                    {
                        yield return $"G1 X{xPos:0.000} Y{yPos:0.000}"; // mm
                        yield return Config.LOFF_Command;
                    }
                    else if (Config.Mode == OperationMode.ZMode)
                    {
                        yield return $"G1 X{xPos:0.000} Y{yPos:0.000} Z{Config.ZON_HeigthMM:0.000}"; // mm
                        yield return $"G1 Z{Config.ZOFF_HeigthMM:0.000}"; // mm
                    }
                    toolStatus = false;
                }

                // avançar a quantidade de mm
                yPos += 1 / Config.PointsPerMM;
            }

            yield break;
        }

        private void getAscii(bool[,] map)
        {
            StringBuilder sb = new StringBuilder();

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    sb.Append(map[x, y] ? "x" : " ");
                }
                sb.Append(Environment.NewLine);
            }

            string txt = sb.ToString();

            File.WriteAllText("out.txt", txt);
        }
    }
}
