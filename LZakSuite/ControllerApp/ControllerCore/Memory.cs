using ControllerApp.Enums;
using ControllerApp.EventArgs;
using ControllerApp.Helpers;
using ControllerApp.Interfaces;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace ControllerApp.ControllerCore
{
    public class Memory
    {
        // configuration for operation
        public readonly IControllerConfig Config;

        // instance lock for writting on console in orderly manner
        public object LockObj;

        // Writer instance. Semaphore is enabled
        public readonly ConsoleHelper ConsoleWriter;

        public string GCODE { get; set; }

        // defines if is manual intervention or GCODE-oriented movements
        public bool ManualOperation { get; internal set; }

        // main serial port object for communication
        public SerialPort Serial { get; }

        // remembers if serial was open or is first connection, in case of disconnection
        public bool WasSerialOpen { get; set; }

        // connection tries counter
        public int CurrentSerialTryCount { get; set; }

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
        public MoveSpeed MotorSpeed { get; set; }
        

        // refers to the speed selected to move motors on manual override
        public MoveSpeed ManualSpeed { get; set; } // defines the speed for manual movement

        // exception cases
        public bool Emergency { get; set; }
        public EmergencyReasons EmergencyReason { get; set; }
        public bool Alarm { get; set; }
        public AlarmReasons AlarmReason { get; set; }

        public Memory(IControllerConfig Config)
        {
            this.Config = Config;
            LockObj = new();
            ManualOperation = false;
            ConsoleWriter = new(LockObj);
            ShutdownToken = new();
            Motor = new(this);

            KeyPressed = 0x00;
            WroteNothingLastRound = false;

            Serial = new SerialPort(Config.SerialCOMPort,
                                    Config.SerialBaudRate,
                                    Config.SerialParity,
                                    Config.SerialDataBits,
                                    Config.SerialStopBits);

            Serial.RtsEnable = Config.EnableRts;
            Serial.DtrEnable = Config.EnableDtr;

            CurrentSerialTryCount = 1;

            MotorSpeed = Config.DefaultMotorSpeed;
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

        /// <summary>
        /// Call for this method if you want some message to be sent out of the software.
        /// </summary>
        /// <param name="e">Arguments class.</param>
        /// <param name="sender">The sender of the current message.</param>
        public void BroadcastMessage(/*MessageOutputEventArgs e*/) 
        {
            // TODO call events
        }
    }
}
