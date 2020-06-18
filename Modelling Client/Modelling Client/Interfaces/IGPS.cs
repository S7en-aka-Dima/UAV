using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelling_Client.Interfaces
{
    interface IGPS
    {
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
        double SpeedX { get; set; }
        double SpeedY { get; set; }
        double SpeedZ { get; set; }
        double AccelX { get; set; }
        double AccelY { get; set; }
        double AccelZ { get; set; }
    }
}
