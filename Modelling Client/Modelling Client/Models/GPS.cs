using Modelling_Client.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Modelling_Client.Models
{
    [Obsolete("Желательно избежать использование этого класса, так как есть структура Settings хранящая ту же информацию")]
    public class GPS : IGPS
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double SpeedX { get; set; }
        public double SpeedY { get; set; }
        public double SpeedZ { get; set; }
        public double AccelX { get; set; }
        public double AccelY { get; set; }
        public double AccelZ { get; set; }
    }
}
