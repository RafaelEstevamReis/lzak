using FTS.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

Console.CursorVisible = false;
bool suspendDraw = false;

CancellationTokenSource cancellationSource = new();
var tk = cancellationSource.Token;

float stepSizeMM = 2;
PointI myLastDrawX = new PointI();


updateAsync();

// Início
Memory.Instance.SetAlarm(AlarmReasons.UnkownCurrentLocation);

while (!tk.IsCancellationRequested)
{
    var k = Console.ReadKey(true);

    switch (k.Key)
    {
        case ConsoleKey.Escape:
            cancellationSource.Cancel();
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
            move(0, +stepSizeMM);
            break;
        case ConsoleKey.DownArrow:
            move(0, -stepSizeMM);
            break;
        case ConsoleKey.LeftArrow:
            move(-stepSizeMM, 0);
            break;
        case ConsoleKey.RightArrow:
            move(+stepSizeMM, 0);
            break;

        case ConsoleKey.Enter:
            suspendDraw = true;
            Console.CursorVisible = true;
            Console.SetCursorPosition(0, 0);
            var line = Console.ReadLine();
            var parts = line.Split(':');
            Console.CursorVisible = false;
            suspendDraw = false;

            moveTo(int.Parse(parts[0]), int.Parse(parts[1]));
            break;
    }
}

void moveTo(float X, float Y)
{
    var relX = X - Memory.Instance.PositionMM.X;
    var relY = Y - Memory.Instance.PositionMM.Y;
    move(relX, relY);
}
void move(float X, float Y)
{
    if (Memory.Instance.Alarm || Memory.Instance.Emergency) return;

    int babySize = (int)Math.Ceiling(Math.Sqrt(X * X + Y * Y) / stepSizeMM);

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
    if (Memory.Instance.PositionMM.X > Cunfiguration.Instance.WorkspaceMM.X)
        Memory.Instance.SetAlarm(AlarmReasons.MoveOutOfBounds);
    if (Memory.Instance.PositionMM.X < 0)
        Memory.Instance.SetAlarm(AlarmReasons.MoveOutOfBounds);

    if (Memory.Instance.PositionMM.Y > Cunfiguration.Instance.WorkspaceMM.Y)
        Memory.Instance.SetAlarm(AlarmReasons.MoveOutOfBounds);
    if (Memory.Instance.PositionMM.Y < 0)
        Memory.Instance.SetAlarm(AlarmReasons.MoveOutOfBounds);

    Memory.Instance.Idle = !Memory.Instance.Moving;
}

void draw()
{
    if (suspendDraw) return;

    var mem = Memory.Instance;
    var cfg = Cunfiguration.Instance;
    int left = 0;
    int top = 0;
    left += 62;
    top += 2;

    PointF pos = mem.PositionMM;

    // 
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
        //                                      sim, ta cagado, nao me deixaram corrigir :'(
        //                                      Tava divertido enquanto era só escolher valores
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