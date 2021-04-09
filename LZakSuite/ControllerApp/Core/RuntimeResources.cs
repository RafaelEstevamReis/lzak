using ControllerApp.MathResources;
using System;
using System.Text;
using System.Threading;

namespace ControllerApp.Core
{
    public class RuntimeResources
    {
        /// <summary>
        /// Try to restore serial connection to the firmware if it has been lost.
        /// </summary>
        /// <param name="memory">The main software memory instance.</param>
        public static void SerialKeepAlive(Memory memory)
        {
            int currentTry = 1;
            bool wasOpen = false;
            Point statusPosition = new Point(60, 20);
            int cleanLen = 55;

            string connectedMsg = $"Serial open at ({memory.Config.SerialCOMPort}).";

            while (true)
            {
                if (memory.ShutdownToken.IsCancellationRequested) return;

                if (memory.Serial.IsOpen) continue;

                string connectionMsg = $"Trying connectio to {memory.Config.SerialCOMPort} ({currentTry}/{memory.Config.MaxSerialReconnectTries})...";
                if (wasOpen)
                {
                    connectionMsg = $"Connection lost to ({memory.Config.SerialCOMPort}). Retrying ({currentTry}/{memory.Config.MaxSerialReconnectTries})...";

                    if (memory.ResetFirmwareKeyPressed)
                    {
                        connectionMsg = $"Firmware reset by user ({memory.Config.SerialCOMPort}). Reconnecting ({currentTry}/{memory.Config.MaxSerialReconnectTries})...";
                    }
                }

                try
                {
                    memory.ConsoleWriter.Write(connectionMsg, statusPosition, cleanBeforeWrite: cleanLen);
                    memory.Serial.Open();
                    wasOpen = true;
                    memory.ConsoleWriter.Write(connectedMsg,
                                               statusPosition,
                                               cleanBeforeWrite: cleanLen);
                    memory.ClearEmergency();
                    currentTry = 1;
                }
                catch { currentTry++; }

                if (currentTry >= memory.Config.MaxSerialReconnectTries) memory.ShutdownToken.Cancel();

                if (!memory.Serial.IsOpen) Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Enables keyboard interaction during a lengthy movement operation.
        /// </summary>
        /// <param name="memory">The main software memory instance.</param>
        /// <param name="token">Cancellation token for killing this execution once movement stops.</param>
        public static void KeyboardListen(Memory memory, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (!Console.KeyAvailable) continue;

                var k = Console.ReadKey(true).Key;
                if (k == memory.Config.Reset) memory.ResetFirmwareKeyPressed = true;
                if (k == memory.Config.Halt) memory.HaltKeyPressed = true;
            }
        }

        /// <summary>
        /// Listens for incoming transmissions from firmware and enqueues it to message pool.
        /// </summary>
        /// <param name="memory">The main software memory instance.</param>
        public static void SerialListen(Memory memory)
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

                            memory.LogMessageStore.Enqueue(message);
                        }
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Gets input from user and operates the motors as needed.
        /// </summary>
        /// <param name="memory">The main software memory instance.</param>
        public static void ManualOperation(Memory memory)
        {
            memory.KeyPressed = memory.Config.NoOperation;
            if (Console.KeyAvailable) memory.KeyPressed = Console.ReadKey(true).Key;

            // nothing is happening, no key was pressed
            if (memory.KeyPressed == memory.Config.NoOperation)
            {
                if (!memory.WroteNothingLastRound) memory.LogMessageStore.Enqueue($"STOPPED");
                memory.WroteNothingLastRound = true;
            }

            if (memory.KeyPressed != memory.Config.NoOperation)
            {
                memory.WroteNothingLastRound = false;
            }

            // stop program
            if (memory.KeyPressed == memory.Config.End)
            {
                memory.ShutdownToken.Cancel();
                memory.LogMessageStore.Enqueue("STOPPING");
                Thread.Sleep(3000);
            }

            // for resetting firmware in case of need
            if (memory.KeyPressed == memory.Config.Reset)
            {
                memory.Motor.RequestFirmwareReset();
            }

            // disable movement via key arrows
            if (memory.KeyPressed == memory.Config.DisableManualOperation)
            {
                memory.ManualOperation = false;
            }

            // activate movement via key arrows
            if (memory.KeyPressed == memory.Config.EnableManualOperation)
            {
                memory.ManualOperation = true;
            }

            // move motor Y+
            if (memory.KeyPressed == memory.Config.Up)
            {
                memory.LogMessageStore.Enqueue("Y+");
                memory.Motor.Move(0, +memory.Config.StepsPerMillimiter_Y);
            }

            // move motor Y-
            if (memory.KeyPressed == memory.Config.Down)
            {
                memory.LogMessageStore.Enqueue("Y-");
                memory.Motor.Move(0, -memory.Config.StepsPerMillimiter_Y);
            }

            // move motor X-
            if (memory.KeyPressed == memory.Config.Left)
            {
                memory.LogMessageStore.Enqueue("X-");
                memory.Motor.Move(-memory.Config.StepsPerMillimiter_X, 0);
            }

            // move motor X+
            if (memory.KeyPressed == memory.Config.Right)
            {
                memory.LogMessageStore.Enqueue("X+");
                memory.Motor.Move(+memory.Config.StepsPerMillimiter_X, 0);
            }
        }

        /// <summary>
        /// Operates machine with GCODE instructions.
        /// </summary>
        /// <param name="memory">The main software memory instance.</param>
        public static void ProcessGCODE(Memory memory)
        {
            // processar gcode:
            // 1. checa alarmes
            // 2. go home.
            // 3. start sending commands.
            throw new NotImplementedException();
        }
    }
}

