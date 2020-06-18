using Modelling_Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelling_Client.Models
{
    public class Iteration : BaseViewModel
    {
        private ObservableCollection<UAVBase> uavs;

        public Iteration()
        {
            uavs = null;
        }
        public Iteration(ObservableCollection<UAVBase> uavs)
        {
            this.uavs = new ObservableCollection<UAVBase>();

            foreach (var uav in uavs)
            {
                var UAV = new UAVBase(uav.Settings);
                this.uavs.Add(UAV);

                var curUAV = this.uavs.Count - 1;
                foreach (var segment in uav.Route)
                    this.uavs[curUAV].Route.Add(segment);
            }
        }

        public ObservableCollection<UAVBase> UAVs
        {
            get => uavs;
            set { uavs = value; OnPropertyChanged(); }
        }
    }
}
