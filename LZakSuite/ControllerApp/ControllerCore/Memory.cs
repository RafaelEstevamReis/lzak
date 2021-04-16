using ControllerApp.Enums;
using ControllerApp.MathResources;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace ControllerApp.ControllerCore
{
    public class Memory
    {
        public readonly Config Config;
        public SerialPort Serial { get; }
        public BinaryWriter BinWriter { get; set; }
        public CancellationTokenSource ShutdownToken;
        public bool StopKeyPressed; 
        public bool ResetFirmwareKeyPressed; 
        public bool Moving { get; set; } 
        public int PositionSteps_X { get; set; }
        public int PositionSteps_Y { get; set; }
        public Motor Motor { get; private set; }
        public MoveSpeed MotorSpeed { get; set; }
        public PointF DestinationPosition { get; set; }
        public bool Emergency { get; set; }
        public EmergencyReasons EmergencyReason { get; set; }
        public bool Alarm { get; set; }
        public AlarmReasons AlarmReason { get; set; }

        public Memory(Config Config)
        {
            this.Config = Config;
            ShutdownToken = new();
            Motor = new(this);
            Serial = new SerialPort(Config.SerialCOMPort,
                                    Config.SerialBaudRate,
                                    Config.SerialParity,
                                    Config.SerialDataBits,
                                    Config.SerialStopBits);

            Serial.RtsEnable = Config.EnableRts;
            Serial.DtrEnable = Config.EnableDtr;
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
    }
}
