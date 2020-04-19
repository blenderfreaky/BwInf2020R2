using ModernWpf.Controls;

namespace Afg3Abbiegen.GUI
{
    using Microsoft.Win32;
    using ModernWpf;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public Map Map { get; private set; }

        public IReadOnlyList<Vector2Int> ShortestPath { get; private set; }
        public int ShortestPathTurns { get; private set; }
        public float ShortestPathLength { get; private set; }

        public IReadOnlyList<Vector2Int> BilalsPath { get; private set; }
        public int BilalsPathTurns { get; private set; }
        public float BilalsPathLength { get; private set; }

        private const int _scale = 50;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void LoadMap_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Map Files|*.txt",
                CheckFileExists = true,
                CheckPathExists = true,
                ValidateNames = true,
                Multiselect = false,
                Title = "Select a file containing a map."
            };

            if (dialog.ShowDialog() == true)
            {
                Map = Map.FromText(File.ReadAllLines(dialog.FileName));

                Task.Run(UpdateMap);
            }
        }

        private void _propertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            Task.Run(UpdateBilalsPath);
        }

        private void UpdateMap()
        {
            _propertyChanged(nameof(Map));
            Dispatcher.Invoke(DrawMap);

            UpdateShortestPath();
            UpdateBilalsPath();
        }

        private void DrawMap()
        {
            MapCanvas.Children.Clear();

            if (Map == null) return;

            foreach (var street in Map.Streets)
            {
                DrawLine(MapCanvas, Brushes.White, 1, street.Start, street.End);
            }

            DrawDot(MapCanvas, Brushes.Green, 10, Map.Start);
            DrawDot(MapCanvas, Brushes.Red, 10, Map.End);
        }

        private void UpdateShortestPath()
        {
            if (Map == null) return;

            ShortestPath = Map.ShortestPath(out var shortestPathLength).ToList();
            ShortestPathTurns = ShortestPath.CountTurns();
            ShortestPathLength = shortestPathLength;

            _propertyChanged(nameof(ShortestPath));
            _propertyChanged(nameof(ShortestPathTurns));
            _propertyChanged(nameof(ShortestPathLength));

            Dispatcher.Invoke(DrawShortestPath);
        }

        private void DrawShortestPath()
        {
            ShortestPathCanvas.Children.Clear();

            if (ShortestPath == null) return;

            for (int i = 0; i < ShortestPath.Count - 1; i++)
            {
                DrawLine(ShortestPathCanvas, Brushes.Red, 2, ShortestPath[i], ShortestPath[i + 1]);
            }
        }

        private void UpdateBilalsPath()
        {
            if (Map == null) return;

            var factor = 0f;
            Dispatcher.Invoke(() => factor = (float)PathLengthFactor.Value);

            BilalsPath = Map.BilalsPath(ShortestPathTurns, ShortestPathLength * factor, out var bilalsPathTurns, out var bilalsPathLength)?.ToList();
            BilalsPathTurns = bilalsPathTurns;
            BilalsPathLength = bilalsPathLength;

            _propertyChanged(nameof(BilalsPath));
            _propertyChanged(nameof(BilalsPathTurns));
            _propertyChanged(nameof(BilalsPathLength));

            Dispatcher.Invoke(DrawBilalsPath);
        }

        private void DrawBilalsPath()
        {
            BilalsPathCanvas.Children.Clear();

            if (BilalsPath == null) return;

            for (int i = 0; i < BilalsPath.Count - 1; i++)
            {
                DrawLine(BilalsPathCanvas, Brushes.Green, 3, BilalsPath[i], BilalsPath[i + 1]);
            }
        }

        /// <summary>
        /// Converts world coordinates to screen coordinates.
        /// </summary>
        /// <param name="vec">The <see cref="Vector2Int"/> to convert.</param>
        /// <returns>The screen coordinates.</returns>
        private Vector2Int WorldToScreen(Vector2Int vec) =>
            new Vector2Int((vec.X - Map.Min.X) * _scale, (Map.Max.Y - vec.Y) * _scale);

        /// <summary>
        /// Draws a line on <paramref name="canvas"/>.
        /// </summary>
        /// <param name="canvas">The <see cref="Canvas"/> to draw on.</param>
        /// <param name="brush">The stroke brush of the line.</param>
        /// <param name="strokeThickness">The stroke thickness of the line.</param>
        /// <param name="start">The start of the line in world coordinates.</param>
        /// <param name="end">The end of the line in world coordinates.</param>
        private void DrawLine(Canvas canvas, Brush brush, float strokeThickness, Vector2Int start, Vector2Int end)
        {
            var startScreen = WorldToScreen(start);
            var endScreen = WorldToScreen(end);

            canvas.Children.Add(new Line
            {
                X1 = startScreen.X,
                Y1 = startScreen.Y,
                X2 = endScreen.X,
                Y2 = endScreen.Y,

                Stroke = brush,
                StrokeThickness = strokeThickness,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,

                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
            });
        }

        /// <summary>
        /// Draws a dot on <paramref name="canvas"/>.
        /// </summary>
        /// <param name="canvas">The <see cref="Canvas"/> to draw on.</param>
        /// <param name="brush">The fill brush of the dot.</param>
        /// <param name="strokeThickness">The thickness of the dot.</param>
        /// <param name="position">The position of the dot in world coordinates.</param>
        private void DrawDot(Canvas canvas, Brush brush, float strokeThickness, Vector2Int position)
        {
            var positionScreen = WorldToScreen(position);

            canvas.Children.Add(new Ellipse
            {
                Margin = new Thickness(positionScreen.X - (strokeThickness / 2), positionScreen.Y - (strokeThickness / 2), 0, 0),
                Width = strokeThickness,
                Height = strokeThickness,

                Fill = brush,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            });
        }
    }
}
