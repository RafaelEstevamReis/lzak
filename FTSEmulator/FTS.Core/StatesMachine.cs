using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace FTS.Core
{
    public class StatesMachine
    {
        static bool SuspendDraw = false;
        static CancellationTokenSource CancellationSource = new();

        public static void Run()
        {
            Console.CursorVisible = false;
            var tk = CancellationSource.Token;
            PointI myLastDrawX = new PointI();

            // mantém a interface atualizando
            updateAsync();

            // Início
            Memory.Instance.SetAlarm(AlarmReasons.UnkownCurrentLocation);

            Movement movement = new(Configuration.Instance);

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
                    case ConsoleKey.H:
                        Memory.Instance.PositionMM = new PointF(); // 0,0
                        if (Memory.Instance.Alarm)
                        {
                            if ((int)Memory.Instance.AlarmReason / 100 == 1)
                            {
                                Memory.Instance.ClearAlarm();
                            }
                        }
                        break;

                    case ConsoleKey.UpArrow:
                        movement.Move(0, +Configuration.Instance.StepSizeMM);
                        break;
                    case ConsoleKey.DownArrow:
                        movement.Move(0, -Configuration.Instance.StepSizeMM);
                        break;
                    case ConsoleKey.LeftArrow:
                        movement.Move(-Configuration.Instance.StepSizeMM, 0);
                        break;
                    case ConsoleKey.RightArrow:
                        movement.Move(+Configuration.Instance.StepSizeMM, 0);
                        break;

                    case ConsoleKey.Enter:
                        Point destination = getCustomPointFromInput(); 
                        movement.MoveTo(destination.X, destination.Y);
                        break;
                }
            }

            static Point getCustomPointFromInput()
            {
                SuspendDraw = true;

                Console.CursorVisible = true;
                Console.SetCursorPosition(0, 0);
                Console.Write(new string(' ', 20));
                Console.SetCursorPosition(0, 0);
                
                var line = Console.ReadLine();
                var parts = line.Split(':');
                Console.CursorVisible = false;
                SuspendDraw = false;

                Console.SetCursorPosition(0, 0);
                Console.Write(new string(' ', 20));
                Console.SetCursorPosition(0, 0);

                return new Point(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]));

                //throw new NotImplementedException("Faz essa parte.");
            }

            async void updateAsync()
            {
                while (!tk.IsCancellationRequested)
                {
                    await Task.Delay(10);
                    checkAlarm();
                    draw();
                }
            }

            void checkAlarm()
            {
                if (Memory.Instance.PositionMM.X > Configuration.Instance.WorkspaceMM.X)
                    Memory.Instance.SetAlarm(AlarmReasons.MoveOutOfBounds);
                if (Memory.Instance.PositionMM.X < 0)
                    Memory.Instance.SetAlarm(AlarmReasons.MoveOutOfBounds);

                if (Memory.Instance.PositionMM.Y > Configuration.Instance.WorkspaceMM.Y)
                    Memory.Instance.SetAlarm(AlarmReasons.MoveOutOfBounds);
                if (Memory.Instance.PositionMM.Y < 0)
                    Memory.Instance.SetAlarm(AlarmReasons.MoveOutOfBounds);

                Memory.Instance.Idle = !Memory.Instance.Moving;
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

                PointF pos = mem.PositionMM;

                SetCursorPosition(myLastDrawX);
                Console.Write(" ");

                Console.SetCursorPosition(left, top++);
                Console.Write($"X: {pos.X}   ");
                Console.SetCursorPosition(left, top++);
                Console.Write($"Y: {pos.Y}   ");

                Console.SetCursorPosition(left, top++);

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
                Console.SetCursorPosition(left, top++);
                Console.Write($"XC: {posTelaReal.X}   ");
                Console.SetCursorPosition(left, top++);
                Console.Write($"YC: {posTelaReal.Y}   ");


                SetCursorPosition(posTelaReal);
                myLastDrawX = posTelaReal;
                Console.Write("X");

                Console.SetCursorPosition(0, 25);
                if (mem.Idle) Console.Write("[IDLE]");
                else Console.Write("[----]");

                Console.SetCursorPosition(10, 25);
                if (mem.Moving) Console.Write("[MOVE]");
                else Console.Write("[----]");

                Console.SetCursorPosition(20, 25);
                if (mem.Engraving) Console.Write("[ENGR]");
                else Console.Write("[----]");

                Console.SetCursorPosition(30, 25);
                if (mem.Alarm) Console.Write($"[ALRM:{(int)mem.AlarmReason}]");
                else Console.Write("[--------]");
            }
            void SetCursorPosition(PointI pos)
            {
                try
                {
                    Console.SetCursorPosition(pos.X, pos.Y);
                }
                catch { }
            }
        }

        
    }
}
