using Modelling_Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Modelling_Client.Views;

using Path = System.Windows.Shapes.Path;

namespace Modelling_Client.Views
{
    /// <summary>
    /// Логика взаимодействия для PrimaryRadar.xaml
    /// </summary>
    public partial class PrimaryRadar : Window
    {
        public int ScaleIndex = 4;
        public double ViewPointX = 0;
        public double ViewPointY = 0;
        public double ViewPointX1 = 0;
        public double ViewPointY1 = 0;
        public double ViewPointX2 = 0;
        public double ViewPointY2 = 0;
        List<Line> RadarGrid = new List<Line>();
        List<Double> Scales = new List<Double>();
        private bool isBotton = false;
        private PrimaryRadar RadarView;

        public double X0
        {
            get => Math.Round(Radar.ActualWidth / 2 - ViewPointX * Scales[ScaleIndex]); // Add check for null obj
        }
        public double Y0
        {
            get => Math.Round(Radar.ActualHeight / 2 + ViewPointY * Scales[ScaleIndex]);
        }

        public PrimaryRadar()
        {
            InitializeComponent();
            ScalesAdd();
        }

        private void DisplayGridLine(Line line)
        {
            if (line.X1 <= Radar.ActualWidth && line.X2 <= Radar.ActualWidth && line.X1 >= 0 && line.X2 >= 0)
                if (line.Y1 <= Radar.ActualHeight && line.Y2 <= Radar.ActualHeight && line.Y1 >= 0 && line.Y2 >= 0)
                {
                    Radar.Children.Add(line);
                    RadarGrid.Add(line);
                }
        }

        private void ScalesAdd()
        {
            Scales.Add(0.1);
            Scales.Add(0.2);
            Scales.Add(0.35);
            Scales.Add(0.5);
            Scales.Add(1);
            Scales.Add(2);
            Scales.Add(3.5);
            Scales.Add(5);
            Scales.Add(10);
        }

        public void DisplayRadarGrid()
        {
            double centerX = X0;
            double centerY = Y0;
            int step = (int)Math.Round(50 * Scales[ScaleIndex]);
            if (Scales[ScaleIndex] == 0.2)
                step = 50 * 2;
            if (Scales[ScaleIndex] == 0.1)
                step = 50 * 1;

            foreach (UIElement obj in RadarGrid)
            {
                Radar.Children.Remove(obj);
            }
            RadarGrid.Clear();

            Brush color = Brushes.Black;
            int axiesThickness = 3;

            // Вертикальные линии
            // упростить код для снижения памяти
            // зная актуальные, размеры окна делим их на два и делим на шаг ==> узнаём количество помещающихся линий
            // запускаем цикл (пока i < крличества помещающихся линий) подставляем i в формулу с x ==> (i * x)
            // отрисовываем по две линии  (с противоположными положениями от оси)
            var line = new Line() { X1 = centerX, X2 = centerX, Y1 = 0, Y2 = Radar.ActualHeight, Stroke = Brushes.Black, StrokeThickness = axiesThickness };
            DisplayGridLine(line);
            for (double x = centerX + step; x < Radar.ActualWidth; x += step)
            {
                line = new Line() { X1 = x, X2 = x, Y1 = 0, Y2 = Radar.ActualHeight, Stroke = Brushes.Black };
                DisplayGridLine(line);
            }
            for (double x = centerX - step; x > 0; x -= step)
            {
                line = new Line() { X1 = x, X2 = x, Y1 = 0, Y2 = Radar.ActualHeight, Stroke = Brushes.Black };
                DisplayGridLine(line);
            }

            // Горизонтальные линии
            line = new Line() { X1 = 0, X2 = Radar.ActualWidth, Y1 = centerY, Y2 = centerY, Stroke = Brushes.Black, StrokeThickness = axiesThickness };
            DisplayGridLine(line);
            for (double y = centerY + step; y < Radar.ActualHeight; y += step)
            {
                line = new Line() { X1 = 0, X2 = Radar.ActualWidth, Y1 = y, Y2 = y, Stroke = Brushes.Black };
                DisplayGridLine(line);
            }
            for (double y = centerY - step; y > 0; y -= step)
            {
                line = new Line() { X1 = 0, X2 = Radar.ActualWidth, Y1 = y, Y2 = y, Stroke = Brushes.Black };
                DisplayGridLine(line);
            }
        }

        private void RadarSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DisplayRadarGrid();
        }

        // добавить блок для масштабирования, обработка колёсика мышка.
        // Масштабирование на колёсико

        private void RadarMouseWheel(object sender, MouseWheelEventArgs e)
        {
            int sign = Math.Sign(e.Delta);

            if (sign > 0 && ScaleIndex + 1 < Scales.Count)
            {
                ScaleIndex = ScaleIndex + 1;
                DisplayRadarGrid();
            }

            else if (sign < 0 && ScaleIndex - 1 >= 0)
            {
                ScaleIndex = ScaleIndex - 1;
                DisplayRadarGrid();
            }
        }

        // добавить передвижение камеры
        private void RadarMouseBottonDown(object sender, MouseButtonEventArgs e)
        {
            isBotton = true;
            Point MoveRadar1 = Mouse.GetPosition(Radar);
            ViewPointX1 = MoveRadar1.X;
            ViewPointY1 = MoveRadar1.Y;
        }

        private void RadarMouseBottonUp(object sender, MouseButtonEventArgs e)
        {
            isBotton = false;
        }

        private void RadarMouseFocusRadar(object sender, MouseEventArgs e)
        {
            if (isBotton == true)
            {
                Point MoveRadar2 = Mouse.GetPosition(Radar);
                ViewPointX2 = MoveRadar2.X;
                ViewPointY2 = MoveRadar2.Y;

                ViewPointX = ViewPointX1 - ViewPointX2;
                ViewPointY = ViewPointY1 - ViewPointY2;
                DisplayRadarGrid();
            };
        }

        // круг БПЛА //
        private void circleUAV(double Radius, double X, double Y/*, double Color */ /* нужен ', double Scale' для регулирования размера ???*/)
        {
            var dot = new Ellipse { Width = 2 * /*uavs[i].Settings.*/Radius, Height = 2 * /*uavs[i].Settings.*/Radius/*, Fill = uavs[i].Color*/ };
            dot.SetValue(PrimaryRadar.LeftProperty, /*uavs[i].Settings.*/X - /*uavs[i].Settings.*/Radius);
            dot.SetValue(PrimaryRadar.TopProperty, /*uavs[i].Settings.*/Y - /*uavs[i].Settings.*/Radius);
            Radar.Children.Add(dot);
        }

        // рисуем все БПЛА
        private void DrawAllUAVS()
        {

        }
    }
}
