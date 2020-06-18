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
    public class Iteration 
    {
        private ObservableCollection<UAVBase> uavs;

        public Iteration()
        {
            uavs = null;
        }
    }
}
