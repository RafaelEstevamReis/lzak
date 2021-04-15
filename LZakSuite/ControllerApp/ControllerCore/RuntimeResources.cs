using ControllerApp.Enums;
using ControllerApp.Helpers;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace ControllerApp.ControllerCore
{
    public class RuntimeResources
    {
        /// <summary>
        /// Try to restore serial connection to the firmware if it has been lost.
        /// </summary>
        /// <param name="memory">The main software memory instance.</param>
        public static void SerialKeepAlive(Memory memory)
        {
            string connectedMsg = $"Serial open at ({memory.Config.SerialCOMPort}).";

            while (!memory.ShutdownToken.IsCancellationRequested)
            {
                if (memory.ShutdownToken.IsCancellationRequested) return;
                if (memory.Serial.IsOpen) return;

                string connectionMsg = $"Trying connectio to {memory.Config.SerialCOMPort} " +
                                       $"({memory.CurrentSerialTryCount}/" +
                                       $"{memory.Config.MaxSerialReconnectTries})...";

                if (memory.WasSerialOpen)
                {
                    connectionMsg = $"Connection lost to ({memory.Config.SerialCOMPort}). " +
                                    $"Retrying ({memory.CurrentSerialTryCount}/" +
                                    $"{memory.Config.MaxSerialReconnectTries})...";

                    if (memory.ResetFirmwareKeyPressed)
                    {
                        connectionMsg = $"Firmware reset by user at ({memory.Config.SerialCOMPort}). " +
                                        $"Reconnecting ({memory.CurrentSerialTryCount}/" +
                                        $"{memory.Config.MaxSerialReconnectTries})...";
                    }
                }

                try
                {
                    // TODO sends out connectionMsg via sink
                    memory.Serial.Open();
                    memory.WasSerialOpen = true;

                    // TODO sends out connectedMsg via sink
                    memory.ClearEmergency();
                    memory.CurrentSerialTryCount = 1;
                }
                catch { memory.CurrentSerialTryCount++; }

                if (memory.CurrentSerialTryCount >= memory.Config.MaxSerialReconnectTries)
                {
                    // TODO sends out
                    memory.ShutdownToken.Cancel();
                }

                if (!memory.Serial.IsOpen)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// Listens for incoming transmissions from firmware and enqueues it to message pool.
        /// </summary>
        /// <param name="memory">The main software memory instance.</param>
        public static void SerialListener(Memory memory)
        {
            byte[] buffer = new byte[256];

            while (!memory.ShutdownToken.IsCancellationRequested)

            {
                if (memory.Serial.IsOpen)
                {
                    try
                    {
                        if (memory.Serial.Read(buffer, 0, buffer.Length) > 0)
                        {
                            var message = Encoding.UTF8.GetString(buffer);
                            message = message.Replace("\0", "");

                            var split = message.Split("\r\n");

                            foreach (var item in split) { } // TODO make this method output to message sink
                                                            // memory.LogMessageQueue.Enqueue(message);
                        }
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Enables keyboard interaction during a lengthy movement operation.
        /// </summary>
        /// <param name="memory">The main software memory instance.</param>
        /// <param name="token">Cancellation token for killing this execution once movement stops.</param>
        public static void KeyboardListener(Memory memory)
        {
            // starts with no operation
            memory.KeyPressed = memory.Config.NoOperation;

            // gets key
            if (Console.KeyAvailable) memory.KeyPressed = Console.ReadKey(true).Key;

            // nothing is happening, no key was pressed 
            // TODO Log something like "stopped" cos nothing is happening
            if (memory.KeyPressed == memory.Config.NoOperation)
            {
                if (!memory.WroteNothingLastRound) memory.WroteNothingLastRound = true;
            }

            // just to not spit that much "im doing nothing" messages on message pool
            if (memory.KeyPressed != memory.Config.NoOperation) memory.WroteNothingLastRound = false;

            // from now on, just a lot of commands.

            // is reset key pressed?
            if (memory.KeyPressed == memory.Config.Reset) memory.ResetFirmwareKeyPressed = true;

            // is halt key pressed?
            if (memory.KeyPressed == memory.Config.Halt) memory.HaltKeyPressed = true;

            // sets motors to faster speed
            if (memory.KeyPressed == memory.Config.MotorSpeedUpKey)
            {
                switch (memory.MotorSpeed)
                {
                    case MoveSpeed.Slow:
                        memory.MotorSpeed = MoveSpeed.Medium;
                        break;
                    case MoveSpeed.Medium:
                        memory.MotorSpeed = MoveSpeed.Fast;
                        break;
                }
            }

            // sets motors to slower speed
            if (memory.KeyPressed == memory.Config.MotorSpeedDownKey)
            {
                switch (memory.MotorSpeed)
                {
                    case MoveSpeed.Fast:
                        memory.MotorSpeed = MoveSpeed.Medium;
                        break;
                    case MoveSpeed.Medium:
                        memory.MotorSpeed = MoveSpeed.Slow;
                        break;
                }
            }

            // stops program
            if (memory.KeyPressed == memory.Config.End) memory.ShutdownToken.Cancel(); //  TODO Log a bye bye message

            // for resetting firmware in case of need
            if (memory.KeyPressed == memory.Config.Reset) memory.Motor.RequestFirmwareReset();

            // disable movement via key arrows
            if (memory.KeyPressed == memory.Config.DisableManualOperation) memory.ManualOperation = false;

            // activate movement via key arrows
            if (memory.KeyPressed == memory.Config.EnableManualOperation) memory.ManualOperation = true;

            // move motor Y+ //  TODO Log "Y+";
            if (memory.KeyPressed == memory.Config.Up) memory.Motor.Move(0, +memory.Config.StepsPerMillimiter_Y);

            // move motor Y- //  TODO Log "Y-";
            if (memory.KeyPressed == memory.Config.Down) memory.Motor.Move(0, -memory.Config.StepsPerMillimiter_Y);

            // move motor X- // TODO Log "X-";
            if (memory.KeyPressed == memory.Config.Left) memory.Motor.Move(-memory.Config.StepsPerMillimiter_X, 0);

            // move motor X+ //  TODO Log "X+";
            if (memory.KeyPressed == memory.Config.Right) memory.Motor.Move(+memory.Config.StepsPerMillimiter_X, 0);
        }

        /// <summary>
        /// Gets input from user and operates the motors as needed.
        /// </summary>
        /// <param name="memory">The main software memory instance.</param>
        public static void ManualOperation(Memory memory) => KeyboardListener(memory);

        /// <summary>
        /// Operates machine with GCODE instructions.
        /// </summary>
        /// <param name="memory">The main software memory instance.</param>
        public static void ProcessGCODE(Memory memory)
        {
            var gcode = memory.GCODE
                              .Split(Environment.NewLine);

            foreach (var l in gcode)
            {
                var G = getInstruction(l, CommandType.G);
                var M = getInstruction(l, CommandType.M);
                float X = getInstruction(l, CommandType.X).StringToFloat();
                float Y = getInstruction(l, CommandType.Y).StringToFloat();

                G = G;

            }
        }

        /// <summary>
        /// Gets values from GCODE type instruction.
        /// </summary>
        /// <param name="line">Gets the line to process.</param>
        /// <param name="type">Gets the type of GCODE.</param>
        /// <returns></returns>
        public static string getInstruction(string line, CommandType type)
        {
            var split = line.Split(" ");

            switch (type)
            {
                case CommandType.M:
                    return split.Where(o => o.StartsWith("M"))
                             .FirstOrDefault()?
                             .Replace("M", "");
                case CommandType.X:
                    return split.Where(o => o.StartsWith("X"))
                             .FirstOrDefault()?
                             .Replace("X", "");
                case CommandType.Y:
                    return split.Where(o => o.StartsWith("Y"))
                             .FirstOrDefault()?
                             .Replace("Y", "");
                case CommandType.Z:
                    return split.Where(o => o.StartsWith("Z"))
                             .FirstOrDefault()?
                             .Replace("Z", "");
                default:
                    return split.Where(o => o.StartsWith("G"))
                             .FirstOrDefault();
            }
        }
    }
}
