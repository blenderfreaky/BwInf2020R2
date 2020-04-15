using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Afg3Abbiegen.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Map Map { get; }

        public MainWindow()
        {
            InitializeComponent();

            Map = Map.FromText(File.ReadAllLines("../../../../../examples/abbiegen2.txt"));
        }

        private void MapCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            MapCanvas.Children.Clear();

            var minX = Map.Streets.Min(x => Math.Min(x.Start.X, x.End.X));
            var minY = Map.Streets.Min(x => Math.Min(x.Start.Y, x.End.Y));
            var maxX = Map.Streets.Max(x => Math.Max(x.Start.X, x.End.X));
            var maxY = Map.Streets.Max(x => Math.Max(x.Start.Y, x.End.Y));
            var width = maxX - minX;
            var height = maxY - minY;
            var scale = Math.Min(MapCanvas.ActualWidth / width, MapCanvas.ActualHeight / height);
            var rand = new Random();
            foreach (var street in Map.Streets)
            {
                MapCanvas.Children.Add(new Line
                {
                    X1 = (street.Start.X - minX) * scale,
                    Y1 = (street.Start.Y - minY) * scale,
                    X2 = (street.End.X - minX) * scale,
                    Y2 = (street.End.Y - minY) * scale,

                    StrokeThickness = 2,
                    Stroke = new SolidColorBrush(Color.FromArgb(255, (byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255))),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                });
            }
        }
    }
}
