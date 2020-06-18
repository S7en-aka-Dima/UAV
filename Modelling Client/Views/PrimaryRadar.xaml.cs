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

using Path = System.Windows.Shapes.Path;

namespace Modelling_Client.Views
{
    /// <summary>
    /// Логика взаимодействия для PrimaryRadar.xaml
    /// </summary>
    public partial class PrimaryRadar : Window
    {
        public double Scale = 1;
        public double ViewPointX = 0;
        public double ViewPointY = 0;
        List<Line> RadarGrid = new List<Line>();

        double X0
        {
            get => Math.Round(Radar.ActualWidth / 2 - ViewPointX * Scale); // Add check for null obj
        }
        double Y0
        {
            get => Math.Round(Radar.ActualHeight / 2 + ViewPointY * Scale);
        }

        public PrimaryRadar()
        {
            InitializeComponent();
            //DisplayPolygonGrid();
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

        public void DisplayPolygonGrid()
        {
            double centerX = X0;
            double centerY = Y0;
            int step = (int)Math.Round(50 * Scale);
            if (Scale == 0.2)
                step = 50 * 2;
            if (Scale == 0.1)
                step = 50 * 1;

            Brush color = Brushes.Black;
            int axiesThickness = 6;

            // Вертикальные линии
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
    }
}
