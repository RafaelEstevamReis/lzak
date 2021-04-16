using HostApp.HostCore;
using System;
using System.Drawing;

namespace HostApp.Interfaces
{
    public interface IEffect
    {
        event EventHandler<PercentageEventArgs> Progress;
        Image Process(Image image);
    }
}
