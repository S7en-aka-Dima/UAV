using DevExpress.Mvvm.Native;

using Modelling_Client.Models.Перечисления;
using Modelling_Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Modelling_Client.Models
{
    public class UAVBase : BaseViewModel
    {
        private int id;
        private DangerLevel dangerLevel = DangerLevel.SafeLevel;
        private UAVSettings settings;
        private ObservableCollection<RouteSegment> route;
        private Color color = Color.FromArgb(26, 0, 0, 0);
        private RouteSegment currentSegment;

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
    }
}
