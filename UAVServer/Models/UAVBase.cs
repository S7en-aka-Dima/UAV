using Modelling_Client.Models.Перечисления;
using Modelling_Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace Modelling_Client.Models
{
    public class UAVBase : BaseViewModel
    {
        [DataMember]
        private int
            id,
            dangerID,
            dangerClientID;

        private DangerLevel dangerLevel = DangerLevel.SafeLevel;
        private UAVSettings settings;
        private List<RouteSegment> route;
        private Color color = Color.FromArgb(26, 0, 0, 0);
        private RouteSegment currentSegment;

        bool canChange = true;

        public UAVBase()
        {
            settings = new UAVSettings();
            route = new List<RouteSegment> { new RouteSegment() };
            currentSegment = route[0];
        }
        public UAVBase(UAVSettings settings)
        {
            this.settings = settings;
            route = new List<RouteSegment> { new RouteSegment() };
            currentSegment = route[0];
        }
        public UAVBase(UAVSettings settings, List<RouteSegment> routeSegments)
        {
            this.settings = settings;
            this.route = routeSegments;
            currentSegment = route == null || route.Count < 1 ? new RouteSegment() : route[0];
        }

        [DataMember]
        public int ClientID
        {
            get => id;
            set { id = value;OnPropertyChanged(); }
        }
        [DataMember]
        public int DangerID
        {
            get => dangerID;
            set { dangerID = value; OnPropertyChanged(); }
        }
        [DataMember]
        public int DangerClientID
        {
            get => dangerClientID;
            set { dangerClientID = value; OnPropertyChanged(); }
        }
        [DataMember]
        public bool CanChange
        {
            get => canChange;
            set { canChange = value; OnPropertyChanged(); }
        }
        [DataMember]
        public DangerLevel DangerLevel
        {
            get => dangerLevel;
            set { dangerLevel = value;OnPropertyChanged(); }
        }
        [DataMember]
        public UAVSettings Settings
        {
            get => settings;
            set { settings = value; OnPropertyChanged(); }
        }
        [DataMember]
        public List<RouteSegment> Route
        {
            get => route;
            set { route = value; OnPropertyChanged(); }
        }
        [DataMember]
        public Color Color
        {
            get => color;
            set { color = value; OnPropertyChanged(); }
        }
        [DataMember]
        public RouteSegment CurrentSegment
        {
            get => currentSegment;
            set { currentSegment = value; OnPropertyChanged(); OnPropertyChanged("Route"); }
        }
    }
}
