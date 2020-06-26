using DevExpress.Mvvm.Native;

using Modelling_Client.Models.Перечисления;
using Modelling_Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SUAVBase = Modelling_Client.UAVServiceHosting.UAVBase;
using SRouteSegment = Modelling_Client.UAVServiceHosting.RouteSegment;
using SUAVSettings = Modelling_Client.UAVServiceHosting.UAVSettings;
using SDangerLevel = Modelling_Client.UAVServiceHosting.DangerLevel;
using DangerLevel = Modelling_Client.Models.Перечисления.DangerLevel;

namespace Modelling_Client.Models
{
    public class UAVBase : BaseViewModel
    {
        private int
            id,
            dangerID,
            dangerClientID;

        private DangerLevel dangerLevel = DangerLevel.SafeLevel;
        private UAVSettings settings;
        private ObservableCollection<RouteSegment> route;
        private Color color = Color.FromArgb(26, 0, 0, 0);
        private RouteSegment currentSegment;

        bool canChange = true;

        public UAVBase()
        {
            settings = new UAVSettings();
            route = new ObservableCollection<RouteSegment> { new RouteSegment() };
            currentSegment = route[0];
        }
        public UAVBase(UAVSettings settings)
        {
            this.settings = settings;
            route = new ObservableCollection<RouteSegment> { new RouteSegment() };
            currentSegment = route[0];
        }
        public UAVBase(UAVSettings settings, ObservableCollection<RouteSegment> routeSegments)
        {
            this.settings = settings;
            this.route = routeSegments;
            currentSegment = route == null || route.Count < 1 ? new RouteSegment() : route[0];
        }

        public int ClientID
        {
            get => id;
            set { id = value;OnPropertyChanged(); }
        }
        public int DangerID
        {
            get => dangerID;
            set { dangerID = value; OnPropertyChanged(); }
        }
        public int DangerClientID
        {
            get => dangerClientID;
            set { dangerClientID = value; OnPropertyChanged(); }
        }
        public bool CanChange
        {
            get => canChange;
            set { canChange = value; OnPropertyChanged(); }
        }
        public DangerLevel DangerLevel
        {
            get => dangerLevel;
            set { dangerLevel = value;OnPropertyChanged(); }
        }
        public UAVSettings Settings
        {
            get => settings;
            set { settings = value; OnPropertyChanged(); }
        }
        public ObservableCollection<RouteSegment> Route
        {
            get => route;
            set { route = value; OnPropertyChanged(); }
        }
        public Color Color
        {
            get => color;
            set { color = value; OnPropertyChanged(); }
        }
        public RouteSegment CurrentSegment
        {
            get => currentSegment;
            set { currentSegment = value; OnPropertyChanged(); OnPropertyChanged("Route"); }
        }

        public static implicit operator UAVBase(SUAVBase uav) => ConverterUAVClasses.Convert(uav) as UAVBase;
        public static explicit operator SUAVBase(UAVBase uav) => ConverterUAVClasses.Convert(uav) as SUAVBase;
    }
}
