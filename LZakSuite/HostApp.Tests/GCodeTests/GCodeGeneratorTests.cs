using HostApp.HostCore;
using System;
using Xunit;

namespace HostApp.Tests.GCodeTests
{
    public class GCodeGeneratorTests
    {
        [Fact]
        public void GCodeTeste_Empty()
        {
            bool[,] map = new bool[10, 10];

            var gcode = new GCODEProcessor(new HostConfig()).ToGCODE(map);

            Assert.Equal("", gcode);
        }
        [Fact]
        public void GCodeTeste_Line()
        {
            bool[,] map = new bool[10, 10];
            map[3, 0] = true;
            map[4, 0] = true;
            map[5, 0] = true;
            map[6, 0] = true;

            var gcode = new GCODEProcessor(new HostConfig()).ToGCODE(map);

            Assert.Equal(@"G1 X3 Y0
M3
G1 X7 Y0
M5
", gcode);
        }
        [Fact]
        public void GCodeTeste_Full()
        {
            bool[,] map = new bool[5, 5];
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++) map[x, y] = true;
            }
            
            var gcode = new GCODEProcessor(new HostConfig()).ToGCODE(map);

            Assert.Equal(@"G1 X0 Y0
M3
G1 X5 Y0
M5
G1 X0 Y1
M3
G1 X5 Y1
M5
G1 X0 Y2
M3
G1 X5 Y2
M5
G1 X0 Y3
M3
G1 X5 Y3
M5
G1 X0 Y4
M3
G1 X5 Y4
M5
", gcode);
        }

        [Fact]
        public void GCodeTeste_BoxZMode()
        {
            bool[,] map = new bool[10, 10];
            for (int y = 2; y < 8; y++)
            {
                map[3, y] = true;
                if (y == 2 || y == 7)
                {
                    map[4, y] = true;
                    map[5, y] = true;
                    map[6, y] = true;
                }
                map[7, y] = true;
            }
            // 2 => de meio em meio mm
            var gcode = new GCODEProcessor(new HostConfig() 
            { Mode = OperationMode.ZMode, PointsPerMM = 1 })
            .ToGCODE(map);

            Assert.Equal(@"G1 X3 Y2 Z1.200
G1 Z0
G1 X8 Y2 Z0
G1 Z1.200
G1 X3 Y3 Z1.200
G1 Z0
G1 X4 Y3 Z0
G1 Z1.200
G1 X7 Y3 Z1.200
G1 Z0
G1 X8 Y3 Z0
G1 Z1.200
G1 X3 Y4 Z1.200
G1 Z0
G1 X4 Y4 Z0
G1 Z1.200
G1 X7 Y4 Z1.200
G1 Z0
G1 X8 Y4 Z0
G1 Z1.200
G1 X3 Y5 Z1.200
G1 Z0
G1 X4 Y5 Z0
G1 Z1.200
G1 X7 Y5 Z1.200
G1 Z0
G1 X8 Y5 Z0
G1 Z1.200
G1 X3 Y6 Z1.200
G1 Z0
G1 X4 Y6 Z0
G1 Z1.200
G1 X7 Y6 Z1.200
G1 Z0
G1 X8 Y6 Z0
G1 Z1.200
G1 X3 Y7 Z1.200
G1 Z0
G1 X8 Y7 Z0
G1 Z1.200
", gcode);
        }

        [Fact]
        public void GCodeTeste_HalfBoxLaserMode()
        {
            bool[,] map = new bool[10, 10];
            for (int y = 2; y < 8; y++)
            {
                map[3, y] = true;                
                if (y == 2 || y == 7)
                {
                    map[4, y] = true;
                    map[5, y] = true;
                    map[6, y] = true;
                }
                map[7, y] = true;
            }
            // 2 => de meio em meio mm
            var gcode = new GCODEProcessor(new HostConfig() { PointsPerMM = 1 })
            .ToGCODE(map);

            Assert.Equal(@"G1 X3 Y2
M3
G1 X8 Y2
M5
G1 X3 Y3
M3
G1 X4 Y3
M5
G1 X7 Y3
M3
G1 X8 Y3
M5
G1 X3 Y4
M3
G1 X4 Y4
M5
G1 X7 Y4
M3
G1 X8 Y4
M5
G1 X3 Y5
M3
G1 X4 Y5
M5
G1 X7 Y5
M3
G1 X8 Y5
M5
G1 X3 Y6
M3
G1 X4 Y6
M5
G1 X7 Y6
M3
G1 X8 Y6
M5
G1 X3 Y7
M3
G1 X8 Y7
M5
", gcode);
        }

        [Fact]
        public void GCODETests_PointsPerMillimetersIsZero()
        {
            GCODEProcessor p = new GCODEProcessor(new HostConfig());
            Assert.Throws<DivideByZeroException>(() => p.Config.PointsPerMM = 0);
        }

        [Fact]
        public void GCODETests_PointsPerMillimetersLessThanZero()
        {
            GCODEProcessor p = new GCODEProcessor(new HostConfig());
            Assert.Throws<ArgumentOutOfRangeException>(() => p.Config.PointsPerMM = -1);
        }
    }
}
