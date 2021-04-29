using ControllerApp.Enums;
using ControllerApp.Helpers;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ControllerApp.ControllerCore
{
    public class Motor
    {
        Memory Memory;
        public Motor(Memory Memory) => this.Memory = Memory;

        // everything here is in mm
        public void MoveTo(float X, float Y)
        {
            int x_step = MathHelper.MillimitersToSteps(X, Memory.Config.StepsPerMillimiter_X);
            int y_step = MathHelper.MillimitersToSteps(Y, Memory.Config.StepsPerMillimiter_Y);

            MoveTo_Steps(x_step, y_step);
        }
        public void MoveTo_Steps(int X, int Y)
        {
            var relX = X - Memory.PositionSteps_X;
            var relY = Y - Memory.PositionSteps_Y;
            Move_Steps(relX, relY);
        }
        public void Move(float X, float Y)
        {
            int x_step = MathHelper.MillimitersToSteps(X, Memory.Config.StepsPerMillimiter_X);
            int y_step = MathHelper.MillimitersToSteps(Y, Memory.Config.StepsPerMillimiter_Y);

            Move_Steps(x_step, y_step);
        }
        public void Move_Steps(int X, int Y)
        {
            // UNITS!
            // mm - user interface values for input
            // steps - instructions to stepper motor
            // screen coordinates - user feedback (UI)

            // current = 0, targets = xy
            int currentX = 0;
            int currentY = 0;

            int dirX = Math.Sign(X);
            int dirY = Math.Sign(Y);

            Memory.Moving = true;

            bool wasRight = false;
            bool wasLeft = false;

            while (currentX != X || currentY != Y)
            {
                if (Memory.Emergency || Memory.Alarm)
                {
                    Memory.Moving = false;
                    break;
                }

                Step sX = Step.None;
                Step sY = Step.None;
                Step sZ = Step.None;

                if (currentX != X)
                {
                    currentX += dirX;
                    Memory.PositionSteps_X += dirX;
                    sX = dirX < 0 ? Step.StepLeft : Step.StepRight;
                }
                if (currentY != Y)
                {
                    currentY += dirY;
                    Memory.PositionSteps_Y += dirY;
                    sY = dirY < 0 ? Step.StepLeft : Step.StepRight;
                }

                switch (Memory.SpindleToggle)
                {
                    case true:
                        if (wasLeft)
                        {
                            sZ = Step.None;
                            break;
                        }
                        sZ = Step.StepLeft;
                        wasLeft = true;
                        wasRight = false;
                        break;
                    case false:
                        if (wasRight)
                        {
                            sZ = Step.None;
                            break;
                        }
                        sZ = Step.StepRight;
                        wasLeft = false;
                        wasRight = true;
                        break;
                }

                SendMoveSignal(sX, sY, sZ);
            }

            Memory.Moving = false;
        }
        void SendMoveSignal(Step x, Step y, Step z)
        {
            try
            {
                // this stream shouldn't be disposed!
                Memory.BinWriter = new BinaryWriter(Memory.Serial.BaseStream);

                byte bX = (byte)x;
                byte bY = (byte)y;
                byte bZ = (byte)z;

                var b = (bX << 4) +
                        (bY << 2) +
                         bZ;

                // first, the binaryWriter has to write current instruction
                // That means turning given pins on.
                Memory.BinWriter.Write((byte)b);

                // always flush before you leave.
                Memory.BinWriter.Flush();

                // then wait a tad... 
                Thread.Sleep(10);

                if (Memory.MotorSpeed == MoveSpeed.Fast) return; // Thread.Sleep(Memory.Config.FastMotorSpeedMs)
                if (Memory.MotorSpeed == MoveSpeed.Medium) Thread.Sleep(Memory.Config.MediumMotorSpeedMs);
                if (Memory.MotorSpeed == MoveSpeed.Slow) Thread.Sleep(Memory.Config.SlowMotorSpeedMs);
            }
            catch
            {
                Memory.SetEmergency(EmergencyReasons.ConnectionLost);
            }
        }
        public void RequestFirmwareReset()
        {
            Memory.ResetFirmwareKeyPressed = true;
            ResetFirmware();
        }
        private void ResetFirmware()
        {
            // Forces disconnection to reset arduino.
            Memory.Serial.Close();
        }
    }
}
