using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerApp.Enums
{
    public enum CommandType
    {
        X, // X axis instruction
        Y, // Y Axis instruction
        Z, // Z axis instruction
        M, // Turn Z on or OFF
        G  // Move command
    }
}
