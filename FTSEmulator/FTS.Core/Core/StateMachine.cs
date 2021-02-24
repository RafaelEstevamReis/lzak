using System;
using System.Threading;
using System.Threading.Tasks;

namespace FTS.Core
{
    public class StateMachine
    {
        static bool SuspendDraw = false;
        static CancellationTokenSource CancellationSource = new();
        static ISerial Serial;
        static object lockObj = new object();
        static ConsoleWriterHelper cWriter;

        #region Events
        // Events!
        private static void Serial_ConnectSuccessfull(SerialConnectEventArgs e)
        {
            cWriter.WriteOnConsole($"SERIAL: Connected on {Serial.Serial.PortName}!", new PointI(64, 0), 50);
        }

        private static void Serial_ConnectFailure(SerialConnectEventArgs e)
        {
            cWriter.WriteOnConsole($"SERIAL: Failed connecting. Message: {e.Exception.Message}.", new PointI(64, 0), 50);
        }

        private static void Serial_TryConnect(SerialConnectEventArgs e)
        {
            cWriter.WriteOnConsole($"SERIAL: Trying to connect on {Serial.Serial.PortName} ({e.CurrentTry})...", new PointI(64, 0), 50);
        }

        private static void Serial_EngravingToggle(SerialCallBackEventArgs e)
        {
            Memory.Instance.SetEmergency(EmergencyReasons.ENDSTOPActivated);
        }
        #endregion

        #region Run
        public static void Run(ISerial serial = null)
        {
            #region Initial Setups
            cWriter = new ConsoleWriterHelper(lockObj);
            Serial = serial;
            if (Serial == null) Serial = new SerialComms();
            Serial.TryConnect += Serial_TryConnect;
            Serial.ConnectFailure += Serial_ConnectFailure;
            Serial.ConnectSuccessful += Serial_ConnectSuccessfull;
            Serial.EngravingToggle += Serial_EngravingToggle;

            if (!Serial.Open())
            {
                throw new Exception($"Serial on {Configuration.Instance.SerialCOMPort} disconnected, cannot proceed.");
            }

            Task.Run(() => Serial.ListenAsync());

            Console.CursorVisible = false;
            var tk = CancellationSource.Token;
            PointI myLastDrawX = new PointI();

            // mantém a interface atualizando
            updateAsync();

            // Início: não sei onde estão os motores.
            Memory.Instance.SetAlarm(AlarmReasons.UnkownCurrentLocation);
            MovementManager movement = new(Serial);

            #endregion

            #region Input loop
            while (!tk.IsCancellationRequested)
            {
                var k = Console.ReadKey(true);

                switch (k.Key)
                {
                    case ConsoleKey.Escape:
                        CancellationSource.Cancel();
                        break;

                    case ConsoleKey.A:
                        Memory.Instance.ClearAlarm();
                        break;
                    case ConsoleKey.E:
                        Memory.Instance.ClearEmergency();
                        break;
                    case ConsoleKey.H:
                        Memory.Instance.PositionSteps_X = 0; // 0,0
                        Memory.Instance.PositionSteps_Y = 0; // 0,0
                        Memory.Instance.PositionSteps_Z = 0; // 0,0

                        if (Memory.Instance.Alarm)
                        {
                            if ((int)Memory.Instance.AlarmReason / 100 == 1)
                            {
                                Memory.Instance.ClearAlarm();
                            }
                        }
                        break;

                    case ConsoleKey.UpArrow:
                        movement.Move(0, +Configuration.Instance.StepsPerMillimiter_Y);
                        break;
                    case ConsoleKey.DownArrow:
                        movement.Move(0, -Configuration.Instance.StepsPerMillimiter_Y);
                        break;
                    case ConsoleKey.LeftArrow:
                        movement.Move(-Configuration.Instance.StepsPerMillimiter_X, 0);
                        break;
                    case ConsoleKey.RightArrow:
                        movement.Move(+Configuration.Instance.StepsPerMillimiter_X, 0);
                        break;

                    case ConsoleKey.Enter:
                        //if (Memory.Instance.Alarm) System.Diagnostics.Debugger.Break();

                        PointF destination = getCustomPointFromInput(out bool valid);
                        if (!valid) continue;
                        Memory.Instance.DestinationPosition = destination;
                        movement.MoveTo(destination.X, destination.Y);
                        break;
                }
            }
            #endregion

            #region User manual positioning
            static PointF getCustomPointFromInput(out bool valid)
            {
                PointI inputPos = Configuration.Instance.UserInputArea;

                SuspendDraw = true;
                Thread.Sleep(100);

                cWriter.WriteOnConsole("Move to position (x:y): ", inputPos, 30, true);
                var line = Console.ReadLine();
                var parts = line.Split(':');
                Console.CursorVisible = false;
                SuspendDraw = false;

                cWriter.CleanLine(inputPos, Console.WindowWidth - 64);

                try
                {
                    var p = new PointF(Convert.ToSingle(parts[0]), Convert.ToSingle(parts[1]));

                    if (p.X > Configuration.Instance.WorkspaceMM.X) p.X = Configuration.Instance.WorkspaceMM.X;
                    if (p.Y > Configuration.Instance.WorkspaceMM.Y) p.Y = Configuration.Instance.WorkspaceMM.Y;

                    valid = true;

                    return p;

                }
                catch { valid = false; }

                return new PointF(0, 0);
            }
            #endregion

            #region AlarmCheck
            void checkAlarm()
            {
                if (Memory.Instance.PositionSteps_X > Configuration.Instance.WorkspaceMM.X)
                {
                    Memory.Instance.SetAlarm(AlarmReasons.MoveOutOfBounds);
                }

                if (Memory.Instance.PositionSteps_X < 0)
                {
                    Memory.Instance.SetAlarm(AlarmReasons.MoveOutOfBounds);
                }

                if (Memory.Instance.PositionSteps_Y > Configuration.Instance.WorkspaceMM.Y)
                {
                    Memory.Instance.SetAlarm(AlarmReasons.MoveOutOfBounds);
                }

                if (Memory.Instance.PositionSteps_Y < 0)
                {
                    Memory.Instance.SetAlarm(AlarmReasons.MoveOutOfBounds);
                }

                Memory.Instance.Idle = !Memory.Instance.Moving;
            }
            #endregion

            #region Draw
            async void updateAsync()
            {
                while (!tk.IsCancellationRequested)
                {
                    await Task.Delay(10);
                    checkAlarm();
                    draw();
                }
            }

            void draw()
            {
                if (SuspendDraw) return;

                var mem = Memory.Instance;
                var cfg = Configuration.Instance;
                int left = 0;
                int top = 0;
                left += 64;
                top += 3;

                PointF pos = new PointF()
                {
                    X = MathHelper.StepsToMillimiters(mem.PositionSteps_X, Configuration.Instance.StepsPerMillimiter_X),
                    Y = MathHelper.StepsToMillimiters(mem.PositionSteps_Y, Configuration.Instance.StepsPerMillimiter_Y)
                };

                cWriter.WriteOnConsole(" ", myLastDrawX, 10);

                // the real position values
                cWriter.WriteOnConsole($"X (value): {pos.X:N1}", new PointI(left, top++), 10);
                cWriter.WriteOnConsole($"Y (value): {pos.Y:N1}", new PointI(left, top++), 10);

                PointF posTela = new PointF()
                {
                    X = MathHelper.Map(0, cfg.WorkspaceMM.X, 0, 59, pos.X),
                    Y = MathHelper.Map(0, cfg.WorkspaceMM.Y, 24, 0, pos.Y)
                };
                PointI posTelaReal = new PointI()
                {
                    X = (int)Math.Round(posTela.X),
                    Y = (int)Math.Round(posTela.Y)
                };

                // the console screen position values
                top++;
                cWriter.WriteOnConsole($"X (console): {posTelaReal.X:N1}", new PointI(left, top++), 20);
                cWriter.WriteOnConsole($"Y (console): {posTelaReal.Y:N1}", new PointI(left, top++), 20);

                // the pen
                cWriter.WriteOnConsole($"o", myLastDrawX = posTelaReal, 0);

                // the hud elements
                drawHudElements(mem, cfg);
            }

            #endregion
        }

        static void drawHudElements(Memory mem, Configuration cfg)
        {
            var uInput = new PointI(cfg.UserInputArea.X - 1, cfg.UserInputArea.Y - 1);
            cWriter.HorizontalLine(new PointI(0, 25), 62);
            cWriter.HorizontalLine(uInput, Console.WindowWidth - uInput.X);
            uInput.Y += 2;
            cWriter.HorizontalLine(new PointI(63, 11), Console.WindowWidth - uInput.X);
            cWriter.VerticalLine(new PointI(cfg.ConsoleTableDimensions.X, 0), Console.WindowHeight);

            if (mem.Moving)
            {
                cWriter.WriteOnConsole($"Moving to ({mem.DestinationPosition.X:N1}, {mem.DestinationPosition.Y:N1})...", cfg.UserInputArea, Console.WindowWidth - 64);
            }
            else
            {
                cWriter.WriteOnConsole("Press Enter for custom move...", cfg.UserInputArea, Console.WindowWidth - 64);
            }

            // the statues on the bottom of the screen
            var bottom = new PointI(cfg.BottomStatuses.X, cfg.BottomStatuses.Y);

            cWriter.WriteOnConsole(mem.Idle ? "[IDLE]" : "[----]", bottom, 0);
            bottom.X += 10;
            cWriter.WriteOnConsole(mem.Moving ? "[MOVE]" : "[----]", bottom, 0);
            bottom.X += 10;
            cWriter.WriteOnConsole(mem.Engraving ? "[ENGR]" : "[----]", bottom, 0);
            bottom.X += 10;
            cWriter.WriteOnConsole(mem.Alarm ? $"[ALRM:{(int)mem.AlarmReason}]" : "[--------]", bottom, 0);
        }
        #endregion
    }
}
