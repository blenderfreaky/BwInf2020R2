using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
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
        public Map Map { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            var args = Environment.GetCommandLineArgs();

            var rootCommand = new RootCommand
            {
                new Option<FileInfo>(
                    "--file",
                    "The file containing the map."),
                new Option<float>(
                    "--weather",
                    "The percentage that the resulting path may be longer than.")
            };

            rootCommand.Description = "My sample app";

            rootCommand.Handler = CommandHandler.Create<FileInfo>(fileInfo =>
            {
                var fileContents = fileInfo.OpenText().ReadToEnd();
                Map = Map.FromText(fileContents.Split(Environment.NewLine));
            });

            rootCommand.Invoke(args);
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
            var scale = 50;

            //var rand = new Random();
            //var col = new SolidColorBrush(Color.FromArgb(255, (byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255)));

            void drawLine(Brush stroke, float strokeThickness, Vector2Int start, Vector2Int end)
            {
                MapCanvas.Children.Add(new Line
                {
                    X1 = (start.X - minX) * scale,
                    Y1 = (maxY - start.Y) * scale,
                    X2 = (end.X - minX) * scale,
                    Y2 = (maxY - end.Y) * scale,

                    Stroke = stroke,
                    StrokeThickness = strokeThickness,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                });
            }

            void drawDot(Brush stroke, float strokeThickness, Vector2Int position)
            {
                MapCanvas.Children.Add(new Ellipse
                {
                    Margin = new Thickness(((position.X - minX) * scale) - (strokeThickness / 2), ((maxY - position.Y) * scale) - (strokeThickness / 2), 0, 0),
                    Width = strokeThickness,
                    Height = strokeThickness,

                    Fill = stroke,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                });
            }

            drawDot(Brushes.DarkRed, 8, Map.Start);
            drawDot(Brushes.DarkOrange, 8, Map.End);

            foreach (var street in Map.Streets)
            {
                drawLine(Brushes.Black, 1, street.Start, street.End);
            }

            var bilalsPath = Map.BilalsPath(1.1f, out var shortestPath, out var shortestPathLength, out var fullTurns, out var fullDistance);

            var shortestPathList = shortestPath.ToList();
            for (int i = 0; i < shortestPathList.Count - 1; i++)
            {
                drawLine(Brushes.Red, 2, shortestPathList[i], shortestPathList[i + 1]);
            }

            var bilalsPathList = bilalsPath.ToList();
            for (int i = 0; i < bilalsPathList.Count - 1; i++)
            {
                drawLine(Brushes.Green, 3, bilalsPathList[i], bilalsPathList[i + 1]);
            }

            foreach (var dbg in Map.DebugDots)
            {
                drawDot(Brushes.Purple, dbg.Item2, dbg.Item1);
            }
            foreach (var dbg in Map.DebugLines)
            {
                drawLine(Brushes.SpringGreen, dbg.Item3, dbg.Item1, dbg.Item2);
            }
        }

        private void LoadMap_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
