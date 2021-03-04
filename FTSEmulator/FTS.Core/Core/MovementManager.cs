using System;
using System.Threading;

namespace FTS.Core
{
    public class MovementManager
    {
        Configuration config;
        ISerial Serial;

        public MovementManager(ISerial Serial)
        {
            config = Configuration.Instance;
            this.Serial = Serial;
        }
        // in mm
        public void MoveTo(float X, float Y)
        {
            int x_step = MathHelper.MillimitersToSteps(X, config.StepsPerMillimiter_X);
            int y_step = MathHelper.MillimitersToSteps(Y, config.StepsPerMillimiter_Y);

            MoveTo_Steps(x_step, y_step);
        }

        public void MoveTo_Steps(int X, int Y)
        {
            var relX = X - Memory.Instance.PositionSteps_X;
            var relY = Y - Memory.Instance.PositionSteps_Y;
            Move_Steps(relX, relY);
        }
        public void Move(float X, float Y)
        {
            int x_step = MathHelper.MillimitersToSteps(X, config.StepsPerMillimiter_X);
            int y_step = MathHelper.MillimitersToSteps(Y, config.StepsPerMillimiter_Y);

            Move_Steps(x_step, y_step);
        }
        public void Move_Steps(int X, int Y)
        {
            // UNITS!
            // mm - user interface values for input
            // steps - instructions to stepper motor
            // screen coordinates - user feedback (UI)

            if (Memory.Instance.Alarm || Memory.Instance.Emergency) return;

            // current = 0
            // target = xy

            int currentX = 0;
            int currentY = 0;

            int dirX = Math.Sign(X);
            int dirY = Math.Sign(Y);

            Memory.Instance.Moving = true;

            while (currentX != X || currentY != Y)
            {
                // has something happened? Immediately stops.
                if (Memory.Instance.Emergency) break;

                // TODO add key or command for triggering emergency stop 
                // gonna do this on latter versions

                Step sX = Step.None;
                Step sY = Step.None;

                if (currentX != X)
                {
                    currentX += dirX;
                    Memory.Instance.PositionSteps_X += dirX;
                    sX = dirX < 0 ? Step.StepLeft : Step.StepRight;
                }
                if (currentY != Y)
                {
                    currentY += dirY;
                    Memory.Instance.PositionSteps_Y += dirY;
                    sY = dirY < 0 ? Step.StepLeft : Step.StepRight;
                }

                Serial.Move(sX, sY, Step.None);
                Thread.Sleep(50);
            }

            Memory.Instance.Moving = false;
        }
    }
}
