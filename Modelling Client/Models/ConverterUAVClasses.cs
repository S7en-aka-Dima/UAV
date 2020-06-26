using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SUAVBase = Modelling_Client.UAVServiceHosting.UAVBase;
using SRouteSegment = Modelling_Client.UAVServiceHosting.RouteSegment;
using SUAVSettings = Modelling_Client.UAVServiceHosting.UAVSettings;
using SDangerLevel = Modelling_Client.UAVServiceHosting.DangerLevel;
using DangerLevel = Modelling_Client.Models.Перечисления.DangerLevel;
using System.Collections.ObjectModel;

namespace Modelling_Client.Models
{
    static class ConverterUAVClasses
    {
        /// <summary>
        /// Для быстрой конвертации типов сервара на клиентский и наоборот
        /// </summary>
        /// <param name="value">Любой класс, который используется классом UAVBase, в том числе и сам класс UAVBase</param>
        /// <remarks>Не может приняимать тип DangerLevel!</remarks>
        /// <returns>Вернёт соответсвующий тип, приведённый в object, или пустой object</returns>
        public static object Convert(object value)
        {
            //if (value is DangerLevel) return ConvertDangerLevel(value as DangerLevel);
            if (value is UAVSettings) return ConvertUAVSettings(value as UAVSettings);
            else if (value is RouteSegment) return ConvertRouteSegment(value as RouteSegment);
            else if (value is ObservableCollection<UAVBase>) return ConvertObservableCollection(value as ObservableCollection<UAVBase>);
            else if (value is SUAVSettings) return ConvertUAVSettings(value as SUAVSettings);
            else if (value is SRouteSegment) return ConvertRouteSegment(value as SRouteSegment);
            else if (value is UAVBase[]) return ConvertObservableCollection(value as SUAVBase[]);
            else if (value is ObservableCollection<RouteSegment>) return ConvertRoute(value as ObservableCollection<RouteSegment>);
            else if (value is SRouteSegment[]) return ConvertRoute(value as SRouteSegment[]);

            return null;
        }
        /// <summary>
        /// Ковретирует серверный тип в клиентский
        /// </summary>
        /// <param name="myDL"></param>
        /// <returns></returns>
        public static SDangerLevel ConvertDangerLevel(DangerLevel myDL)
        {
            SDangerLevel sDangerLevel = SDangerLevel.SafeLevel;

            switch (myDL)
            {
                case DangerLevel.SafeLevel:
                    sDangerLevel = SDangerLevel.SafeLevel;
                    break;

                case DangerLevel.Crash:
                    sDangerLevel = SDangerLevel.Crash;
                    break;
            }

            return sDangerLevel;
        }
        /// <summary>
        /// Ковретирует клиентский тип в серверный
        /// </summary>
        /// <param name="myDL"></param>
        /// <returns></returns>
        public static DangerLevel ConvertDangerLevel(SDangerLevel sDL)
        {
            DangerLevel DangerLevel = DangerLevel.SafeLevel;

            switch (sDL)
            {
                case SDangerLevel.SafeLevel:
                    DangerLevel = DangerLevel.SafeLevel;
                    break;

                case SDangerLevel.Crash:
                    DangerLevel = DangerLevel.Crash;
                    break;
            }

            return DangerLevel;
        }
        /// <summary>
        /// Ковретирует серверный тип в клиентский
        /// </summary>
        /// <param name="myDL"></param>
        /// <returns></returns>
        public static SUAVBase[] ConvertObservableCollection(ObservableCollection<UAVBase> OCUAVB)
        {
            var LUAVB = new List<SUAVBase>();

            foreach (var uav in OCUAVB)
                LUAVB.Add(ConvertUAVBase(uav));

            return LUAVB.ToArray();
        }
        /// <summary>
        /// Ковретирует клиентский тип в серверный
        /// </summary>
        /// <param name="myDL"></param>
        /// <returns></returns>
        public static ObservableCollection<UAVBase> ConvertObservableCollection(SUAVBase[] LUAVB)
        {
            var OCUAVB = new ObservableCollection<UAVBase>();

            foreach (var uav in LUAVB)
                OCUAVB.Add(ConvertUAVBase(uav as SUAVBase));

            return OCUAVB;
        }
        /// <summary>
        /// Ковретирует серверный тип в клиентский
        /// </summary>
        /// <param name="myDL"></param>
        /// <returns></returns>
        public static SUAVBase ConvertUAVBase(UAVBase MyUAV)
        {
            var uav = new SUAVBase
            {
                CanChange = false,
                ClientID = MyUAV.ClientID,
                Color = MyUAV.Color,
                DangerClientID = MyUAV.DangerClientID,
                DangerID = MyUAV.DangerID,
                DangerLevel = ConvertDangerLevel(MyUAV.DangerLevel),
                Settings = ConvertUAVSettings(MyUAV.Settings),
                Route = ConvertRoute(MyUAV.Route),
                CurrentSegment = ConvertRouteSegment(MyUAV.CurrentSegment)
            };

            return uav;
        }
        /// <summary>
        /// Ковретирует клиентский тип в серверный
        /// </summary>
        /// <param name="myDL"></param>
        /// <returns></returns>
        public static UAVBase ConvertUAVBase(SUAVBase MyUAV)
        {
            var uav = new UAVBase
            {
                CanChange = false,
                ClientID = MyUAV.ClientID,
                Color = MyUAV.Color,
                DangerClientID = MyUAV.DangerClientID,
                DangerID = MyUAV.DangerID,
                DangerLevel = ConvertDangerLevel(MyUAV.DangerLevel),
                Settings = ConvertUAVSettings(MyUAV.Settings),
                CurrentSegment = ConvertRouteSegment(MyUAV.CurrentSegment),
                Route = ConvertRoute(MyUAV.Route)
            };

            return uav;
        }
        /// <summary>
        /// Ковретирует серверный тип в клиентский
        /// </summary>
        /// <param name="myDL"></param>
        /// <returns></returns>
        public static SRouteSegment ConvertRouteSegment(RouteSegment segment) => new SRouteSegment
        {
            DiscretionToChange = segment.DiscretionToChange,
            EndPoint = segment.EndPoint,
            StartPoint = segment.StartPoint,
            Speed = segment.Speed,
            SpeedX = segment.SpeedX,
            SpeedY = segment.SpeedY,
            SpeedZ = segment.SpeedZ
        };
        /// <summary>
        /// Ковретирует клиентский тип в серверный
        /// </summary>
        /// <param name="myDL"></param>
        /// <returns></returns>
        public static RouteSegment ConvertRouteSegment(SRouteSegment segment) => new RouteSegment
        {
            DiscretionToChange = segment.DiscretionToChange,
            EndPoint = segment.EndPoint,
            StartPoint = segment.StartPoint,
            Speed = segment.Speed,
            SpeedX = segment.SpeedX,
            SpeedY = segment.SpeedY,
            SpeedZ = segment.SpeedZ
        };
        /// <summary>
        /// Ковретирует серверный тип в клиентский
        /// </summary>
        /// <param name="myDL"></param>
        /// <returns></returns>
        public static SRouteSegment[] ConvertRoute(ObservableCollection<RouteSegment> LUAVB)
        {
            var route = new List<SRouteSegment>();

            foreach (var segment in LUAVB)
                route.Add(ConvertRouteSegment(segment as RouteSegment));

            return route.ToArray();
        }
        /// <summary>
        /// Ковретирует клиентский тип в серверный
        /// </summary>
        /// <param name="myDL"></param>
        /// <returns></returns>
        public static ObservableCollection<RouteSegment> ConvertRoute(Array AUAVB)
        {
            var route = new ObservableCollection<RouteSegment>();

            foreach (var segment in AUAVB)
                route.Add(ConvertRouteSegment(segment as SRouteSegment));

            return route;
        }
        /// <summary>
        /// Ковретирует серверный тип в клиентский
        /// </summary>
        /// <param name="myDL"></param>
        /// <returns></returns>
        public static SUAVSettings ConvertUAVSettings(UAVSettings settings) => new SUAVSettings
        {
            ID = settings.ID,
            MaxSpeed = settings.MaxSpeed,
            MinSpeed = settings.MinSpeed,
            Accel = settings.Accel,
            Speed = settings.Speed,
            Radius = settings.Radius,
            X = settings.X,
            Y = settings.Y,
            Z = settings.Z
        };
        /// <summary>
        /// Ковретирует клиентский тип в серверный
        /// </summary>
        /// <param name="myDL"></param>
        /// <returns></returns>
        public static UAVSettings ConvertUAVSettings(SUAVSettings settings) => new UAVSettings
        {
            ID = settings.ID,
            MaxSpeed = settings.MaxSpeed,
            MinSpeed = settings.MinSpeed,
            Accel = settings.Accel,
            Speed = settings.Speed,
            Radius = settings.Radius,
            X = settings.X,
            Y = settings.Y,
            Z = settings.Z
        };
    }
}
