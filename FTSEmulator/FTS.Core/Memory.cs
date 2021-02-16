namespace FTS.Core
{
    public partial class Memory
    {
        public static Memory Instance { get; } = new Memory();

        public PointF PositionMM { get; set; }
        public PointF FeedRateMmPerMinute { get; set; }

        // Statuses
        public bool Idle { get; set; }
        public bool Moving { get; set; }
        public bool Engraving { get; set; }
        
        public bool Alarm { get; set; }
        public AlarmReasons AlarmReason { get; set; }
        
        public bool Emergency { get; }
        public EmergencyReasons EmergencyReason { get; set; }

        private Memory()
        {
            PositionMM = new PointF();
            FeedRateMmPerMinute = new PointF();
        }

        public void SetAlarm(AlarmReasons reason)
        {
            Alarm = true;
            AlarmReason = reason;
        }

        public void ClearAlarm()
        {
            Alarm = false;
        }
    }
}
