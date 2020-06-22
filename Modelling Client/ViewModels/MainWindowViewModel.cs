using DevExpress.Mvvm;
using System;
using System.Windows.Input;
using System.Windows;
using Modelling_Client.Models;
using System.Collections.ObjectModel;
using Modelling_Client.Models.Перечисления;
using System.Windows.Media;
using Modelling_Client.Views;
using Microsoft.Win32;
using System.Data;
using Modelling_Client.UAVServiceHosting;
using UAVBase = Modelling_Client.Models.UAVBase;
using RouteSegment = Modelling_Client.Models.RouteSegment;
using UAVSettings = Modelling_Client.Models.UAVSettings;
using DangerLevel = Modelling_Client.Models.Перечисления.DangerLevel;
using SUAVBase = Modelling_Client.UAVServiceHosting.UAVBase;
using SRouteSegment = Modelling_Client.UAVServiceHosting.RouteSegment;
using SUAVSettings = Modelling_Client.UAVServiceHosting.UAVSettings;
using SDangerLevel = Modelling_Client.UAVServiceHosting.DangerLevel;

namespace Modelling_Client.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private UAVBase selectedUAV;
        private Modelling modelling;
        private RouteSegment selectedRouteSigment;
        private ObservableCollection<RouteSegment> segments;
        private ObservableCollection<UAVBase> uavs;

        PrimaryRadar primaryRadar;

        private bool myUAV = true;

        public MainWindowViewModel()
        {
            modelling = new Modelling();

            uavs = modelling.AllUAVBases;
            selectedUAV = uavs[0];
            segments = selectedUAV.Route;
        }

        #region Поля
        public bool MyUAV
        {
            get => myUAV;
            set { myUAV = value; OnPropertyChanged(); }
        }
        public Modelling Modelling
        {
            get => modelling;
            set { modelling = value; OnPropertyChanged(); }
        }
        public ObservableCollection<UAVBase> UAVs
        {
            get => uavs;
            set { uavs = value; OnPropertyChanged(); }
        }
        public UAVBase SelectedUAV
        {
            get => selectedUAV;
            set { selectedUAV = value; OnPropertyChanged(); }
        }
        public ObservableCollection<RouteSegment> Route
        {
            get => segments;
            set { segments = value; OnPropertyChanged(); }
        }
        public RouteSegment SelectedRouteSigment
        {
            get => selectedRouteSigment;
            set { selectedRouteSigment = value; OnPropertyChanged(); }
        }
        #endregion

        #region Тест
        public ICommand Test => (new DelegateCommand(TestCommand));
        public ICommand<UAVsWorkMode> Test1 => (new DelegateCommand<UAVsWorkMode>(TestCommand1));

        public void TestCommand()
        {
            string str = string.Empty;
            modelling.ServiceClient.Disconnect(modelling.ThisClientID);

            MessageBox.Show($"{str}!");
        }
        public void TestCommand1(UAVsWorkMode msg)
        {
            MessageBox.Show($"{msg}");
        }
        #endregion

        #region Команды
        public ICommand Open => new DelegateCommand(() =>
        {
            primaryRadar = new PrimaryRadar();
            primaryRadar.Show();
            primaryRadar.DisplayRadarGrid();
            modelling.StartModelling();
        },
            () => primaryRadar == null);
        public ICommand NewModelling => new DelegateCommand(() =>
        {
            primaryRadar.Close();
            primaryRadar = null;
            modelling.Simulate = false;
        },
            () => primaryRadar != null);
        public ICommand<RouteSegment> RemoveRouteSegment => new DelegateCommand<RouteSegment>(DeleteRouteSegment);
        public ICommand<RouteSegment> Change => new DelegateCommand<RouteSegment>(DiscretionToChange);
        public ICommand<UAVBase> ResetSettings => new DelegateCommand<UAVBase>(SetToZero);
        public ICommand<UAVBase> RemoveUAV => new DelegateCommand<UAVBase>(DeleteUAV);
        public ICommand<UAVBase> SelectUAVSettings => new DelegateCommand<UAVBase>(GiveUAVSettings);
        public ICommand<bool> ConnectToServer => new DelegateCommand<bool>(modelling.Connect);
        public ICommand<bool> SaveFile => new DelegateCommand<bool>((par) =>
        {
            modelling.SaveSettingsIntoFile(par);
            Update();
        });
        public ICommand<int> AddOrDeleteUAV => new DelegateCommand<int>(AddOrDeleteUAVs);
        public ICommand SaveIntoExcel => new DelegateCommand(modelling.SaveIntoExcel);
        public ICommand OpenFile => new DelegateCommand(() =>
        {
            modelling.OpenSettingsFile();
            Update();
        });
        public ICommand AddSegment => new DelegateCommand(AddSegmentIntoRout);
        public ICommand ResetAll => new DelegateCommand(BringToCommonStandard);
        public ICommand Exit => new DelegateCommand(() =>
        {
            Application.Current.Shutdown();
        });
        #endregion
        
        #region Свойства
        private void Update()
        {
            uavs = modelling.AllUAVBases;
            selectedUAV = uavs[0];
            SelectedRouteSigment = uavs[0].Route[0];

            OnPropertyChanged("UAVs");
            OnPropertyChanged("SelectedUAV");
            OnPropertyChanged("Route");
            OnPropertyChanged("SelectedRouteSigment");
        }
        private void BringToCommonStandard()
        {
            foreach (var uav in uavs) SetToZero(uav);
        }
        private void GiveUAVSettings(UAVBase uav)
        {
#if DEBUG
            MessageBox.Show(uav.Settings.ID.ToString() + " " + (uav.ClientID==modelling.ThisClientID));
#endif
            selectedUAV = uav;
            OnPropertyChanged("SelectedUAV");
            IsMine();
        }
        private void AddSegmentIntoRout()
        {
            if (selectedUAV.ClientID != modelling.ThisClientID) return;

            if (selectedUAV.Route.Count == 0)
            {
                selectedUAV.Route.Add(new RouteSegment());
                return;
            }

            selectedUAV.Route.Add(new RouteSegment(
                segments[segments.Count - 1].EndX, 0.0, 0.0,
                segments[segments.Count - 1].EndY, 0.0, 0.0,
                segments[segments.Count - 1].EndZ, 0.0, 0.0));

            for(int i = 0; i < selectedUAV.Route.Count - 1; i++)
                selectedUAV.Route[i].DiscretionToChange = false;

            SelectedRouteSigment = selectedUAV.Route[selectedUAV.Route.Count - 1];
            //OnPropertyChanged("SelectedUAV");
        }
        private void SetToZero(UAVBase uav)
        {
            int id = uav.Settings.ID;

            uav.Settings = new UAVSettings();
            uav.Route = new ObservableCollection<RouteSegment> { new RouteSegment() };

            uav.Settings.ID = id;
            uav.DangerLevel = DangerLevel.SafeLevel;
            uav.CurrentSegment = uav.Route[0];

            OnPropertyChanged("UAVs");
        }

        public void DiscretionToChange(RouteSegment segment)
        {
            segment.DiscretionToChange = true;
        }
        public void IsMine()
        {
            myUAV = selectedUAV.ClientID == modelling.ThisClientID;
            OnPropertyChanged("MyUAV");
        }
        public void DeleteRouteSegment(RouteSegment segment)
        {
            if (selectedUAV.ClientID != modelling.ThisClientID) return;

            if (selectedUAV.Route.Count == 1) return;

            int index = selectedUAV.Route.IndexOf(segment);

            selectedUAV.Route.Remove(segment);

            if (index <= selectedUAV.Route.Count && selectedUAV.Route.Count > 1)
            {
                if (index >= 2)
                    selectedUAV.Route[index].StartPoint = selectedUAV.Route[index - 1].EndPoint;
            }

            selectedRouteSigment = selectedUAV.Route[selectedUAV.Route.Count - 1];
            OnPropertyChanged("SelectedRouteSigment");
        }
        public void AddOrDeleteUAVs(int num, UAVsWorkMode mode)
        {
            switch (mode)
            {
                case UAVsWorkMode.Add:
                    for (int i = 0; i < num; i++)
                    {
                        UAVBase uav = new UAVBase();
                        uav.Settings.ID = uavs.Count + 1;
                        uavs.Add(uav);
                        OnPropertyChanged("UAVs");
                        GC.Collect();
                    }
                    break;

                case UAVsWorkMode.Delete:
                    if (uavs.Count <= num && uavs.Count != 0)
                    {
                        for (int i = uavs.Count - 1; uavs.Count > 2; i--)
                            uavs.RemoveAt(i);

                        OnPropertyChanged("UAVs");
                        GC.Collect();
                        return;
                    }
                    else
                    {
                        int maxi = uavs.Count - num > 2 ? uavs.Count - num : 2;
                        for (int i = uavs.Count - 1; i >= maxi; i--) uavs.RemoveAt(i);
                        OnPropertyChanged("UAVs");
                        GC.Collect();
                    }
                    break;
            }
#if DEBUG
            MessageBox.Show($"{GC.GetTotalMemory(true) / 1024 / 1024}");
#endif
        }
        public void AddOrDeleteUAVs(int num)
        {
            if (num > 0)
                for (int i = 0; i < num; i++)
                {
                    UAVBase uav = new UAVBase();
                    uav.ClientID = modelling.ThisClientID;
                    uav.Settings.ID = uavs.Count + 1;
                    uav.Color = modelling.UAVColor;
                    uavs.Add(uav);
                }
            else if (uavs.Count <= Math.Abs(num) && uavs.Count != 0)
            {
                for (int i = uavs.Count - 1; uavs.Count > 2; i--)
                    uavs.RemoveAt(i);
            }
            else
            {
                int maxi = uavs.Count + num > 2 ? uavs.Count + num : 2;
                for (int i = uavs.Count - 1; i >= maxi; i--) uavs.RemoveAt(i);
            }

            selectedUAV = uavs[uavs.Count - 1];

            modelling.PutColor();
            OnPropertyChanged("SelectedUAV");
            OnPropertyChanged("UAVs");
            GC.Collect();

#if DEBUG
            //MessageBox.Show($"{GC.GetTotalMemory(true) / 1024 / 1024}");
#endif
        }
        public void DeleteUAV(UAVBase uav)
        {
            if (uavs.IndexOf(uav) == 0)
            {
                MessageBox.Show("Нельзя удалить этот!");

                return; 
            }

            uavs.Remove(uav);
            OnPropertyChanged();
        }
        #endregion
    }
}