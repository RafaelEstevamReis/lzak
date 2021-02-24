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

        #region Events
        // Events!
        private static void Serial_ConnectSuccessfull(SerialConnectEventArgs e)
        {
            WriteOnConsole($"SERIAL: Connected on {Serial.Serial.PortName}!", new PointI(62, 0), 50);
        }

        private static void Serial_ConnectFailure(SerialConnectEventArgs e)
        {
            WriteOnConsole($"SERIAL: Failed connecting. Message: {e.Exception.Message}.", new PointI(62, 0), 50);
        }

        private static void Serial_TryConnect(SerialConnectEventArgs e)
        {
            WriteOnConsole($"SERIAL: Trying to connect on {Serial.Serial.PortName} ({e.CurrentTry})...", new PointI(62, 0), 50);
        }

        private static void Serial_EngravingToggle(SerialCallBackEventArgs e)
        {
            Memory.Instance.SetEmergency(EmergencyReasons.ENDSTOPActivated);
        }
        #endregion

        #region ConsoleWriter
        // Console Writer, centralized, so onscreen texts aren't going to get messy.
        public static void WriteOnConsole(string message, PointI location, int cleanLen, bool showCursor = false)
        {
            lock (lockObj)
            {
                Console.CursorVisible = showCursor;

                Console.SetCursorPosition(location.X, location.Y);
                Console.Write(new string(' ', cleanLen));
                Console.SetCursorPosition(location.X, location.Y);
                Console.Write(message);

                if (Console.CursorVisible) Console.CursorVisible = false;
            }
        }

        public static void CleanLine(PointI pos, int cleanLen)
        {
            WriteOnConsole(string.Empty, pos, cleanLen);
            Console.SetCursorPosition(pos.X, pos.Y);
        }

        #endregion

        #region Run
        public static void Run(ISerial serial = null)
        {
            #region Initial Setups
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
                        movement.Move(0, +Configuration.Instance.StepsPerMilimiter_Y);
                        break;
                    case ConsoleKey.DownArrow:
                        movement.Move(0, -Configuration.Instance.StepsPerMilimiter_Y);
                        break;
                    case ConsoleKey.LeftArrow:
                        movement.Move(-Configuration.Instance.StepsPerMilimiter_X, 0);
                        break;
                    case ConsoleKey.RightArrow:
                        movement.Move(+Configuration.Instance.StepsPerMilimiter_X, 0);
                        break;

                    case ConsoleKey.Enter:
                        if (Memory.Instance.Alarm) System.Diagnostics.Debugger.Break();

                        PointI destination = getCustomPointFromInput(out bool valid);
                        if (!valid) continue;
                        movement.MoveTo(destination.X, destination.Y);
                        break;
                }

            }
            #endregion

            #region User manual positioning
            static PointI getCustomPointFromInput(out bool valid)
            {
                SuspendDraw = true;
                Thread.Sleep(100);

                WriteOnConsole("Go to (x:y): ", new PointI(0, 0), 20, true);
                var line = Console.ReadLine();
                var parts = line.Split(':');
                Console.CursorVisible = false;
                SuspendDraw = false;

                CleanLine(new PointI(0, 0), 20);
                try
                {
                    var p = new PointI(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]));
                    valid = true;

                    return p;

                }
                catch { valid = false; }

                return new PointI(0, 0);

                // melhorar este input
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
                left += 62;
                top += 2;

                PointF pos = new PointF()
                {
                    X = MathHelper.StepsToMillimiters(mem.PositionSteps_X, 
                                                      Configuration.Instance.StepsPerMilimiter_X),

                    Y = MathHelper.StepsToMillimiters(mem.PositionSteps_Y, 
                                                      Configuration.Instance.StepsPerMilimiter_Y)
                };

                WriteOnConsole(" ", myLastDrawX, 10);

                // the real position values
                WriteOnConsole($"X (value): {pos.X:N1}", new PointI(left, top++), 10);
                WriteOnConsole($"Y (value): {pos.Y:N1}", new PointI(left, top++), 10);

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
                WriteOnConsole($"X (console): {posTelaReal.X:N1}", new PointI(left, top++), 20);
                WriteOnConsole($"X (console): {posTelaReal.Y:N1}", new PointI(left, top++), 20);

                // the pen
                WriteOnConsole($"o", myLastDrawX = posTelaReal, 0);

                // the bottom statuses
                setBottomStatuses(mem);
            }

            #endregion
        }

        static void setBottomStatuses(Memory mem)
        {
            // the statues on the bottom of the screen
            PointI position = new PointI(0, 25);

            WriteOnConsole(mem.Idle ? "[IDLE]" : "[----]", position, 0);
            position.X += 10;
            WriteOnConsole(mem.Moving ? "[MOVE]" : "[----]", position, 0);
            position.X += 10;
            WriteOnConsole(mem.Engraving ? "[ENGR]" : "[----]", position, 0);
            position.X += 10;
            WriteOnConsole(mem.Alarm ? $"[ALRM:{(int)mem.AlarmReason}]" : "[--------]", position, 0);
        }
        #endregion
    }
}
