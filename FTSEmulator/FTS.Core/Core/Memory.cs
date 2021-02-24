using System;
using System.Drawing;

namespace FTS.Core
{
    public partial class Memory
    {
        public static Memory Instance { get; } = new Memory();

        public int PositionSteps_X { get; set; }
        public int PositionSteps_Y { get; set; }
        public int PositionSteps_Z { get; set; }

        public PointF FeedRateMmPerMinute { get; set; }

        public PointF DestinationPosition { get; set; }

        // Statuses
        public bool Idle { get; set; }
        public bool Moving { get; set; }
        public bool Engraving { get; set; }
        
        public bool Alarm { get; set; }
        public AlarmReasons AlarmReason { get; set; }
        
        public bool Emergency { get; set; }
        public EmergencyReasons EmergencyReason { get; set; }

        private Memory()
        {
            PositionSteps_X = 0;
            PositionSteps_Y = 0;
            PositionSteps_Z = 0;

            FeedRateMmPerMinute = new PointF();
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
