using DevExpress.Mvvm;

using Modelling_Client.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Modelling_Client.ViewModels
{
    class SettingsViewModel : BaseViewModel
    {
        private ObservableCollection<Models.UAVSettings> allUAVs = new ObservableCollection<UAVSettings>
        {
            new Models.UAVSettings{ID = 0, Radius = 10, Speed = 5, StartX = 10, StartY = 10, EndX = 2, EndY = 5},
            new Models.UAVSettings{ID = 1, Radius = 10, Speed = 5},
            new Models.UAVSettings{ID = 2, Radius = 15},
            new Models.UAVSettings{ID = 3, Radius = 20},
            new Models.UAVSettings{ID = 4, Radius = 10, Speed = 5},
            new Models.UAVSettings{ID = 5, Radius = 10, Speed = 5},
            new Models.UAVSettings{ID = 6, Radius = 15},
            new Models.UAVSettings{ID = 7, Radius = 20},
            new Models.UAVSettings{ID = 8, Radius = 10, Speed = 5},
            new Models.UAVSettings{ID = 9, Radius = 15}
        };
        private ObservableCollection<UAVBase> uavs = new ObservableCollection<UAVBase>
        {
            new UAVBase(new Models.UAVSettings{ID = 0, Radius = 10, Speed = 5, StartX = 10, StartY = 10, EndX = 2, EndY = 5}),
            new UAVBase(new Models.UAVSettings{ID = 1, Radius = 10, Speed = 5}),
            new UAVBase(new Models.UAVSettings{ID = 2, Radius = 15}),
            new UAVBase(new Models.UAVSettings{ID = 3, Radius = 20}),
            new UAVBase(new Models.UAVSettings{ID = 4, Radius = 10, Speed = 5}),
            new UAVBase(new Models.UAVSettings{ID = 5, Radius = 10, Speed = 5}),
            new UAVBase(new Models.UAVSettings{ID = 6, Radius = 15}),
            new UAVBase(new Models.UAVSettings{ID = 7, Radius = 20}),
            new UAVBase(new Models.UAVSettings{ID = 8, Radius = 10, Speed = 5}),
            new UAVBase(new Models.UAVSettings{ID = 9, Radius = 15})
        };

        private UAVBase selectedUAV;
        private UAVSettings selectedSettings;
        private RouteSegment routeSegment;
        private ObservableCollection<RouteSegment> segments;

        public SettingsViewModel()
        {
            selectedUAV = uavs[0];
            selectedSettings = allUAVs[0];

            segments = new ObservableCollection<RouteSegment>
            {
                new RouteSegment(selectedSettings.StartX, 0.0, 0.0,
                selectedSettings.StartY, 0.0, 0.0,
                selectedSettings.StartZ, 0.0, 0.0),
                new RouteSegment(0.0, selectedSettings.EndX, 0.0,
                0.0, selectedSettings.EndY, 0.0,
                0.0, selectedSettings.EndZ, 0.0)
            };

            segments = new ObservableCollection<RouteSegment>
            {
                new RouteSegment(selectedSettings.StartX, 0.0, 0.0,
                selectedSettings.StartY, 0.0, 0.0,
                selectedSettings.StartZ, 0.0, 0.0),
                new RouteSegment(0.0, selectedSettings.EndX, 0.0,
                0.0, selectedSettings.EndY, 0.0,
                0.0, selectedSettings.EndZ, 0.0)
            };

            //segments[segments.Count - 1].DiscretionToChange = false;
        }

        public ObservableCollection<UAVBase> UAVs
        {
            get => uavs;
            set { uavs = value; OnPropertyChanged(); }
        }
        public ObservableCollection<Models.UAVSettings> AllUAVs
        {
            get => allUAVs;
            set { allUAVs = value; OnPropertyChanged(); }
        }
        public UAVSettings SelectedSettings
        {
            get => selectedSettings;
            set
            {
                selectedSettings = value;
                OnPropertyChanged();
            }
        }
        public RouteSegment RouteSegment
        {
            get => routeSegment;
            set { routeSegment = value; OnPropertyChanged(); }
        }
        public ObservableCollection<RouteSegment> Route
        {
            get => segments;
            set { segments = value; OnPropertyChanged(); }
        }

        public DelegateCommand AddSegment => new DelegateCommand(AddSegmentIntoRout);
        public DelegateCommand<UAVSettings> DelegateCommand => new DelegateCommand<UAVSettings>(GiveNextUAVSettings);
        public DelegateCommand<UAVSettings> SelectUAVSettings => new DelegateCommand<UAVSettings>((settings) =>
        {
            MessageBox.Show(settings.ID.ToString());
            selectedSettings = settings;
            OnPropertyChanged("SelectedSettings");
        });

        private void GiveNextUAVSettings(UAVSettings nextUAVSettings)
        {
            selectedSettings = nextUAVSettings;
        }
        private void AddSegmentIntoRout()
        {
            //  не проходит по условию, не меняет значения в settings
            if (segments[segments.Count - 1].EndX != selectedSettings.EndX) selectedSettings.EndX = segments[segments.Count - 1].EndX;
            if (segments[segments.Count - 1].EndY != selectedSettings.EndY) selectedSettings.EndY = segments[segments.Count - 1].EndY;
            if (segments[segments.Count - 1].EndZ != selectedSettings.EndZ) selectedSettings.EndZ = segments[segments.Count - 1].EndZ;

            segments[segments.Count - 2].DiscretionToChange = false;

            segments.Add(new RouteSegment(segments[segments.Count - 2].EndX, 0.0, 0.0,
                segments[segments.Count - 2].EndY, 0.0, 0.0,
                segments[segments.Count - 2].EndZ, 0.0, 0.0));

            segments.Add(segments[segments.Count - 2]);

            segments.RemoveAt(segments.Count - 3);

#if DeBUG
            MessageBox.Show(string.Format(
                $"Точка начала сегмента \tА({segments[segments.Count - 2].StartX}, {segments[segments.Count - 2].StartY}, {segments[segments.Count - 2].StartZ})\n" +
                $"Точка конца сегмента \tВ({segments[segments.Count - 2].EndPoint[0]},{segments[segments.Count - 2].EndPoint[1]}, {segments[segments.Count - 2].EndPoint[2]})\n" +
                $"Точка начала маршрута\tА({selectedSettings.StartX}, {selectedSettings.StartY}, {selectedSettings.StartZ})\n" +
                $"Точка конца маршрута \tВ({selectedSettings.EndX},{selectedSettings.EndY}, {selectedSettings.EndZ})"));
#endif
        }
    }
}
