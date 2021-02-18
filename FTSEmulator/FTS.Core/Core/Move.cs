using System;
using System.Threading;

namespace FTS.Core
{
    public class Movement
    {
        Configuration config;
        ISerial Serial;

        public Movement(ISerial Serial)
        {
            config = Configuration.Instance;
            this.Serial = Serial;
        }

        public void MoveTo(float X, float Y)
        {
            var relX = X - Memory.Instance.PositionMM.X;
            var relY = Y - Memory.Instance.PositionMM.Y;
            Move(relX, relY);
        }
        public void Move(float X, float Y)
        {
            if (Memory.Instance.Alarm || Memory.Instance.Emergency) return;

            int babySize = (int)Math.Ceiling(Math.Sqrt(X * X + Y * Y) / config.StepSizeMM);

            Memory.Instance.Moving = true;
            float babyX = (float)Math.Round(X / babySize, 2);
            float babyY = (float)Math.Round(Y / babySize, 2);

            for (int i = 0; i < babySize; i++)
            {
                Memory.Instance.PositionMM.X += babyX;
                Memory.Instance.PositionMM.Y += babyY;
                Thread.Sleep(50);
            }
            Memory.Instance.Moving = false;
        }
    }
}
