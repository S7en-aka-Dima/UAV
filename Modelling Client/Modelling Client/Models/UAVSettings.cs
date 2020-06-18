using Modelling_Client.Interfaces;
using Modelling_Client.ViewModels;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Modelling_Client.Models
{
    public class UAVSettings : BaseViewModel, IGPS
    {
        private int id = 1;

        private double
            speed, accel, radius,
            x, speedX, accelX,
            y, speedY, accelY,
            z, speedZ, accelZ;

        /// <summary>
        /// ID БПЛА внутри одной программы
        /// </summary>
        public int ID
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }
        public double Radius
        {
            get => radius;
            set
            {
                radius = value;
                OnPropertyChanged();
            }
        }
        public double Speed
        { 
            get => speed;
            set
            {
                speed = value;
                OnPropertyChanged();
            }
        }
        public double Accel {
            get => accel;
            set
            {
                accel = value;
                OnPropertyChanged();
            }
        }
        public double X
        {
            get => x;
            set
            {
                x = value;
                OnPropertyChanged();
            }
        }
        public double Y
        {
            get => y;
            set
            {
                y = value;
                OnPropertyChanged();
            }
        }
        public double Z
        {
            get => z;
            set
            {
                z = value;
                OnPropertyChanged();
            }
        }
        /*
        /// <summary>
        /// Координата начала маршрута
        /// </summary>
        public double StartX
        {
            get => startX;
            set
            {
                startX = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Координата начала маршрута
        /// </summary>
        public double StartY
        {
            get => startY;
            set
            {
                startY = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Координата начала маршрута
        /// </summary>
        public double StartZ
        {
            get => startZ;
            set
            {
                startZ = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Координата начала маршрута
        /// </summary>
        public double EndX
        {
            get => endX;
            set
            {
                endX = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Координата начала маршрута
        /// </summary>
        public double EndY
        {
            get => endY;
            set
            {
                endY = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Координата начала маршрута
        /// </summary>
        public double EndZ
        {
            get => endZ;
            set
            {
                endZ = value;
                OnPropertyChanged();
            }
        }*/
        //  Может быть не нужно!
        public double SpeedX
        {
            get => speedX;
            set
            {
                speedX = value;
                OnPropertyChanged();
            }
        }
        public double SpeedY
        {
            get => speedY;
            set
            {
                speedY = value;
                OnPropertyChanged();
            }
        }
        public double SpeedZ
        {
            get => speedZ;
            set
            {
                speedZ = value;
                OnPropertyChanged();
            }
        }
        public double AccelX
        {
            get => accelX;
            set
            {
                accelX = value;
                OnPropertyChanged();
            }
        }
        public double AccelY
        {
            get => accelY;
            set
            {
                accelY = value;
                OnPropertyChanged();
            }
        }
        public double AccelZ
        {
            get => accelZ;
            set
            {
                accelZ = value;
                OnPropertyChanged();
            }
        }
    }
}
