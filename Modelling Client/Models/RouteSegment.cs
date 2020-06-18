using Modelling_Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelling_Client.Models
{
    public class RouteSegment : BaseViewModel
    {
        bool discretionToChange = true;

        private double[]
            speed = { 0.0, 0.0, 0.0 },
            startPoint = { 0.0, 0.0, 0.0 },
            endPoint = { 0.0, 0.0, 0.0 };

        public RouteSegment()
        {
            startPoint = new double[] { 0.0, 0.0, 0.0 };
            endPoint = new double[] { 0.0, 0.0, 0.0 };
            speed = new double[] { 0.0, 0.0, 0.0 };
        }
        /// <summary>
        /// Учитываейте, что точки задаются как X, Y, Z!
        /// Скорость по осям равна нулю, по умолчанию
        /// </summary>
        /// <param name="startPoint">X, Y, Z начала сегмента</param>
        /// <param name="endPoint">X, Y, Z конца сегмента</param>
        public RouteSegment(double[] startPoint, double[] endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            speed = new double[] { 0.0, 0.0, 0.0 };
        }
        /// <summary>
        /// Учитываейте, что точки задаются как X, Y, Z!
        /// Скорость по осям равна нулю, по умолчанию
        /// </summary>
        /// <param name="startPoint">X, Y, Z начала сегмента</param>
        /// <param name="xStartCoordinate">X конца сегмента</param>
        /// <param name="yStartCoordinate">Y конца сегмента</param>
        /// <param name="zStartCoordinate">Z конца сегмента</param>
        public RouteSegment(double[] startPoint,
            double xStartCoordinate, double yStartCoordinate, double zStartCoordinate = 0.0)
        {
            this.startPoint = startPoint;
            endPoint = new double[3];
            speed = new double[] { 0.0, 0.0, 0.0 };

            endPoint[0] = xStartCoordinate;
            endPoint[1] = yStartCoordinate;
            endPoint[2] = zStartCoordinate;
        }
        /// <summary>
        /// Учитываейте, что точки задаются как X, Y, Z!
        /// Скорость по осям равна нулю, по умолчанию
        /// </summary>
        /// <param name="xStartCoordinate">X начала сегмента</param>
        /// <param name="xEndCoordinate">Y начала сегмента</param>
        /// <param name="yStartCoordinate">Z начала сегмента</param>
        /// <param name="xStartCoordinate">X конца сегмента</param>
        /// <param name="yStartCoordinate">Y конца сегмента</param>
        /// <param name="zStartCoordinate">Z конца сегмента</param>
        public RouteSegment(double xStartCoordinate, double xEndCoordinate,
            double yStartCoordinate, double yEndCoordinate,
            double zStartCoordinate = 0.0, double zEndCoordinate = 0.0)
        {
            startPoint = new double[3];
            endPoint = new double[3];
            speed = new double[] { 0.0, 0.0, 0.0 };

            startPoint[0] = xStartCoordinate;
            startPoint[1] = yStartCoordinate;
            startPoint[2] = zStartCoordinate;

            endPoint[0] = xEndCoordinate;
            endPoint[1] = yEndCoordinate;
            endPoint[2] = zEndCoordinate;
        }
        /// <summary>
        /// Учитываейте, что точки задаются как X, Y, Z!
        /// </summary>
        /// <param name="startPoint">X, Y, Z начала сегмента</param>
        /// <param name="speed">Скорость по 3-м осям</param>
        /// <param name="xStartCoordinate">X конца сегмента</param>
        /// <param name="yStartCoordinate">Y конца сегмента</param>
        /// <param name="zStartCoordinate">Z конца сегмента</param>
        public RouteSegment(double[] startPoint, double[] speed,
            double xStartCoordinate, double yStartCoordinate, double zStartCoordinate = 0.0)
        {
            this.startPoint = startPoint;
            this.speed = speed;
            endPoint = new double[3];

            endPoint[0] = xStartCoordinate;
            endPoint[1] = yStartCoordinate;
            endPoint[2] = zStartCoordinate;
        }
        /// <summary>
        /// Учитываейте, что точки задаются как X, Y, Z!
        /// </summary>
        /// <param name="startPoint">X, Y, Z начала сегмента</param>
        /// <param name="xStartCoordinate">X конца сегмента</param>
        /// <param name="xSpeed">Скорость по оси X</param>
        /// <param name="yStartCoordinate">Y конца сегмента</param>
        /// <param name="ySteed">Скорость по оси Y</param>
        /// <param name="zStartCoordinate">Z конца сегмента</param>
        /// <param name="zSpeed">Скорость по оси Z</param>
        public RouteSegment(double[] startPoint,
            double xStartCoordinate, double xSpeed,
            double yStartCoordinate, double ySpeed,
            double zStartCoordinate = 0.0, double zSpeed = 0.0)
        {
            speed = new double[3];
            endPoint = new double[3];

            this.startPoint = startPoint;

            endPoint[0] = xStartCoordinate;
            endPoint[1] = yStartCoordinate;
            endPoint[2] = zStartCoordinate;

            speed[0] = xSpeed;
            speed[1] = ySpeed;
            speed[2] = zSpeed;
        }
        /// <summary>
        /// Учитываейте, что точки задаются как X, Y, Z!
        /// </summary>
        /// <param name="xStartCoordinate">X начала сегмента</param>
        /// <param name="xEndCoordinate">X конца сегмента</param>
        /// <param name="xSpeed">Скорость по оси X</param>
        /// <param name="yStartCoordinate">Y начала сегмента</param>
        /// <param name="yEndCoordinate">Y конца сегмента</param>
        /// <param name="ySteed">Скорость по оси Y</param>
        /// <param name="ZStartCoordinate">Z начала сегмента</param>
        /// <param name="zEndCoordinate">Z конца сегмента</param>
        /// <param name="zSpeed">Скорость по оси Z</param>
        public RouteSegment(double xStartCoordinate, double xEndCoordinate, double xSpeed,
            double yStartCoordinate, double yEndCoordinate, double ySpeed,
            double zStartCoordinate = 0.0, double zEndCoordinate = 0.0, double zSpeed = 0.0)
        {
            startPoint = new double[3];
            endPoint = new double[3];
            speed = new double[3];

            startPoint[0] = xStartCoordinate;
            startPoint[1] = yStartCoordinate;
            startPoint[2] = zStartCoordinate;

            endPoint[0] = xEndCoordinate;
            endPoint[1] = yEndCoordinate;
            endPoint[2] = zEndCoordinate;

            speed[0] = xSpeed;
            speed[1] = ySpeed;
            speed[2] = zSpeed;
        }

        public bool DiscretionToChange
        {
            get => discretionToChange;
            set { discretionToChange = value; OnPropertyChanged(); }
        }
        public double[] StartPoint
        {
            get => startPoint;
            set { startPoint = value; OnPropertyChanged(); }
        }
        public double[] EndPoint
        {
            get => endPoint;
            set { endPoint = value; OnPropertyChanged(); }
        }
        public double[] Speed
        {
            get => speed;
            set { speed = value; OnPropertyChanged(); }
        }
        public double StartX
        {
            get => StartPoint[0];
            set { StartPoint[0] = value; OnPropertyChanged("StartPoint"); }
        }
        public double StartY
        {
            get => StartPoint[1];
            set { StartPoint[1] = value; OnPropertyChanged("StartPoint"); }
        }
        public double StartZ
        {
            get => StartPoint[2];
            set { StartPoint[2] = value; OnPropertyChanged("StartPoint"); }
        }
        public double EndX
        {
            get => endPoint[0];
            set { endPoint[0] = value; OnPropertyChanged("EndPoint"); }
        }
        public double EndY
        {
            get => endPoint[1];
            set { endPoint[1] = value; OnPropertyChanged("EndPoint"); }
        }
        public double EndZ
        {
            get => endPoint[2];
            set { endPoint[2] = value; OnPropertyChanged("EndPoint"); }
        }
        public double SpeedX
        {
            get => speed[0];
            set { speed[0] = value; OnPropertyChanged("Speed"); }
        }
        public double SpeedY
        {
            get => speed[1];
            set { speed[1] = value; OnPropertyChanged("Speed"); }
        }
        public double SpeedZ
        {
            get => speed[2];
            set { speed[2] = value; OnPropertyChanged("Speed"); }
        }
    }
}
