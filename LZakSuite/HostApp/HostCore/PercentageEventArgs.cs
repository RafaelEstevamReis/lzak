using System;

namespace HostApp.HostCore
{
    public class PercentageEventArgs : EventArgs
    {
        public int Current { get; set; }
        public int Total { get; set; }

        public double Percentage => (100.0 * Current) / Total;
    }
}
