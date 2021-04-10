using ControllerApp.Enums;
using ControllerApp.Helpers;
using ControllerApp.Interfaces;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace ControllerApp.Core
{
    public class Memory
    {
        // configuration for operation
        public readonly IConfiguration Config;

        // instance lock for writting on console in orderly manner
        public object LockObj;

        // Writer instance. Semaphore is enabled
        public readonly ConsoleHelper ConsoleWriter;

        // message handlers
        public ConcurrentQueue<string> LogMessageQueue { get; set; }

        public string GCODE { get; set; }

        // defines if is manual intervention or GCODE-oriented movements
        public bool ManualOperation { get; internal set; }

        // main serial port object for communication
        public SerialPort Serial { get; }

        // Binary writer for serial comms
        public BinaryWriter BinWriter { get; set; }

        // Cancellation token for every started thread
        public CancellationTokenSource ShutdownToken;

        // current pressed key for menu options
        public ConsoleKey KeyPressed { get; set; }

        // For log purposes, the "no operation" should appear only once on console
        // this ensures console will not be flooded with no key pressed message.
        public bool WroteNothingLastRound { get; set; }

        // keys for halting movement <i>during</i> movement
        public bool HaltKeyPressed; // stops movement, but leave everyting as is
        public bool ResetFirmwareKeyPressed; // resets fw

        // movement-related instance statuses
        public bool Moving { get; set; } // defines is any motor is moving
        public int PositionSteps_X { get; set; }
        public int PositionSteps_Y { get; set; }
        public MotorInterface Motor { get; private set; } // object of motor manipulation

        // refers to the speed selected to move motors on manual override
        public MoveSpeed ManualSpeed { get; set; } // defines the speed for manual movement

        // exception cases
        public bool Emergency { get; set; }
        public EmergencyReasons EmergencyReason { get; set; }
        public bool Alarm { get; set; }
        public AlarmReasons AlarmReason { get; set; }

        public Memory(IConfiguration Config)
        {
            this.Config = Config;
            LockObj = new();
            ManualOperation = false;
            ConsoleWriter = new(LockObj);
            ShutdownToken = new();
            Motor = new(this);

            LogMessageQueue = new();

            KeyPressed = 0x00;
            WroteNothingLastRound = false;

            Serial = new SerialPort(Config.SerialCOMPort,
                                    Config.SerialBaudRate,
                                    Config.SerialParity,
                                    Config.SerialDataBits,
                                    Config.SerialStopBits);

            Serial.RtsEnable = Config.EnableRts;
            Serial.DtrEnable = Config.EnableDtr;
        }

        public void SetAlarm(AlarmReasons reason)
        {
            Alarm = true;
            AlarmReason = reason;
        }
        public void SetEmergency(EmergencyReasons reason)
        {
            Emergency = true;
            EmergencyReason = reason;
        }
        public void ClearAlarm() => Alarm = false;
        internal void ClearEmergency() => Emergency = false;
    }
}
