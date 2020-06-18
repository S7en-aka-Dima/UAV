using Modelling_Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UAVServer
{
    [Serializable]
    [DataContract(Name = "Class")]
    public class TransportClass
    {
        private UAVBase UserUAV;
        private double y;

        public TransportClass() { }

        [DataMember]
        public UAVBase UAV { get => UserUAV; set { UserUAV = value; } }

        public TransportClass(UAVBase uav)
        {
            UserUAV = uav;
        }

        [DataMember]
        public double NewY { get => y; set { y = value; } }
        
        /*
        [DataMember]
        public double NewX { get => UAV.XAffter; set { } }

        [DataMember]
        public double Radius { get => UAV.Radius; set { } }

        [DataMember]
        public double Speed { get => UAV.Speed; set { } }
        */
    }
}
