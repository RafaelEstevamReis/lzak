﻿using ControllerApp.Enums;
using ControllerApp.Interfaces;
using ControllerApp.MathResources;
using System;
using System.IO.Ports;

namespace ControllerApp.ControllerCore
{
    public class ControllerConfig : IControllerConfig
    {
        // serial configs
        public int SerialBaudRate { get; set; }
        public string SerialCOMPort { get; set; }
        public int SerialMaxCnnTries { get; set; }
        public Parity SerialParity { get; set; }
        public int SerialDataBits { get; set; }
        public StopBits SerialStopBits { get; set; }
        public int MaxSerialReconnectTries { get; set; }
        public bool EnableDtr { get; set; }
        public bool EnableRts { get; set; }

        // workspace configs
        // [0,0] is at Lower Left Corner!
        public PointF WorkspaceMM { get; }
        public int StepsPerMillimiter_X { get; set; }
        public int StepsPerMillimiter_Y { get; set; }
        public int StepsPerMillimiter_Z { get; set; }

        // manual movement configs
        public ConsoleKey NoOperation { get; set; }
        public ConsoleKey End { get; set; }
        public ConsoleKey Up { get; set; }
        public ConsoleKey Down { get; set; }
        public ConsoleKey Left { get; set; }
        public ConsoleKey Right { get; set; }
        public ConsoleKey ToolToggle { get; set; }
        public ConsoleKey Halt { get; set; }
        public ConsoleKey Reset { get; set; }
        public ConsoleKey Home { get; set; }
        public ConsoleKey EnableManualOperation { get; set; }
        public ConsoleKey DisableManualOperation { get; set; }

        public int MaxLogLineLength { get; set; }
        public MoveSpeed DefaultMotorSpeed { get; set; }
        public ConsoleKey MotorSpeedUpKey { get; set; }
        public ConsoleKey MotorSpeedDownKey { get; set; }

        public ControllerConfig()
        {
            // serial config
            SerialBaudRate = 9600;
            SerialCOMPort = "COM7";
            SerialMaxCnnTries = 10;
            SerialParity = Parity.None;
            SerialDataBits = 8;
            SerialStopBits = StopBits.One;
            EnableDtr = true;
            EnableRts = true;

            MaxSerialReconnectTries = 10;

            DefaultMotorSpeed = MoveSpeed.Slow;

            // Workspace details
            StepsPerMillimiter_X = 150;
            StepsPerMillimiter_Y = 150;
            StepsPerMillimiter_Z = 1;

            WorkspaceMM = new PointF(100, 100);

            MaxLogLineLength = 50;

            // Manual Control keys setup
            NoOperation = 0x00;
            End = ConsoleKey.Escape;
            Up = ConsoleKey.UpArrow;
            Down = ConsoleKey.DownArrow;
            Left = ConsoleKey.LeftArrow;
            Right = ConsoleKey.RightArrow;
            ToolToggle = ConsoleKey.T;
            Halt = ConsoleKey.H;
            Reset = ConsoleKey.R;
            EnableManualOperation = ConsoleKey.F2;
            DisableManualOperation = ConsoleKey.F1;
            MotorSpeedUpKey = ConsoleKey.PageUp;
            MotorSpeedDownKey = ConsoleKey.PageDown;
        }
    }
}
