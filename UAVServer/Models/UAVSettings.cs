using Modelling_Client.Interfaces;
using Modelling_Client.ViewModels;

using System.Runtime.Serialization;

namespace Modelling_Client.Models
{
    /*
     *  может перенести в сегмент маршрута?
     *  speed - одинакова для всех сегментов?
     *  критическая скорость для всех сегментов одинаковая?
     */
    [DataContract]
    public class UAVSettings : BaseViewModel, IGPS
    {
        private int id = 1;

        private double
            speed, accel, radius,
            x, speedX, accelX,
            y, speedY, accelY,
            z, speedZ, accelZ,
            maxSpeed, minSpeed;

        /// <summary>
        /// ID БПЛА внутри одной программы
        /// </summary>
        [DataMember]
        public int ID
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }
        [DataMember]
        public double Radius
        {
            get => radius;
            set
            {
                radius = value;
                OnPropertyChanged();
            }
        }
        [DataMember]
        public double Speed
        { 
            get => speed;
            set
            {
                speed = value;
                OnPropertyChanged();
            }
        }
        [DataMember]
        public double Accel {
            get => accel;
            set
            {
                accel = value;
                OnPropertyChanged();
            }
        }
        [DataMember]
        public double MaxSpeed
        {
            get => maxSpeed;
            set { maxSpeed = value; OnPropertyChanged(); }
        }
        [DataMember]
        public double MinSpeed
        {
            get => minSpeed;
            set { minSpeed = value; OnPropertyChanged(); }
        }
        [DataMember]
        public double X
        {
            get => x;
            set
            {
                x = value;
                OnPropertyChanged();
            }
        }
        [DataMember]
        public double Y
        {
            get => y;
            set
            {
                y = value;
                OnPropertyChanged();
            }
        }
        [DataMember]
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
        [DataMember]
        public double SpeedX
        {
            get => speedX;
            set
            {
                speedX = value;
                OnPropertyChanged();
            }
        }
        [DataMember]
        public double SpeedY
        {
            get => speedY;
            set
            {
                speedY = value;
                OnPropertyChanged();
            }
        }
        [DataMember]
        public double SpeedZ
        {
            get => speedZ;
            set
            {
                speedZ = value;
                OnPropertyChanged();
            }
        }
        [DataMember]
        public double AccelX
        {
            get => accelX;
            set
            {
                accelX = value;
                OnPropertyChanged();
            }
        }
        [DataMember]
        public double AccelY
        {
            get => accelY;
            set
            {
                accelY = value;
                OnPropertyChanged();
            }
        }
        [DataMember]
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
