using ControllerApp.Enums;
using ControllerApp.Helpers;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ControllerApp.ControllerCore
{
    public class MotorInterface
    {
        Memory Memory;
        public MotorInterface(Memory Memory) => this.Memory = Memory;

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

        int stepXcount = 0;
        int stepYcount = 0;
        public void Move_Steps(int X, int Y)
        {
            // UNITS!
            // mm - user interface values for input
            // steps - instructions to stepper motor
            // screen coordinates - user feedback (UI)

            if (Memory.Alarm || Memory.Emergency) return;

            // reset and stop solution for movement.
            Memory.HaltKeyPressed = false;
            Memory.ResetFirmwareKeyPressed = false;

            // current = 0, targets = xy
            int currentX = 0;
            int currentY = 0;

            int dirX = Math.Sign(X);
            int dirY = Math.Sign(Y);

            Memory.Moving = true;

            while (currentX != X || currentY != Y)
            {
                RuntimeResources.KeyboardListener(Memory);

                // has something happened? Immediately stops.
                if (Memory.Emergency) break;
                if (Memory.Alarm) break;
                if (Memory.ShutdownToken.IsCancellationRequested) break;

                // was reset key pressed?
                if (Memory.ResetFirmwareKeyPressed)
                {
                    Memory.Motor.ResetFirmware();
                    break;
                }

                // was stop key pressed (no reset)
                if (Memory.HaltKeyPressed) return;

                Step sX = Step.None;
                Step sY = Step.None;

                if (currentX != X)
                {
                    currentX += dirX;
                    Memory.PositionSteps_X += dirX;
                    sX = dirX < 0 ? Step.StepLeft : Step.StepRight;
                    stepXcount++;
                }
                if (currentY != Y)
                {
                    currentY += dirY;
                    Memory.PositionSteps_Y += dirY;
                    sY = dirY < 0 ? Step.StepLeft : Step.StepRight;
                    stepYcount++;
                }

                SendMoveSignal(sX, sY, Step.None);
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
                if (Memory.MotorSpeed == MoveSpeed.Fast) return;
                if (Memory.MotorSpeed == MoveSpeed.Medium) Thread.Sleep(1);
                if (Memory.MotorSpeed == MoveSpeed.Slow) Thread.Sleep(150);
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
            // Since theres a thread running the method at
            // <ControllerApp.Helpers.RuntimeHelper.SerialKeepAlive()>
            // for restabilishing connection in case of failure,
            // just disconnecting is enough. It'll reset and get it up again.

            Memory.Serial.Close();
        }
    }
}
