using Downloader;
using Imya.UI.Utils;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace Imya.UI.Controls
{
    /// <summary>
    /// Interaktionslogik für DownloadInfoDisplay.xaml
    /// </summary>
    public partial class DownloadInfoDisplay : UserControl, INotifyPropertyChanged
    {
        #region DependencyProperties
        public static readonly DependencyProperty DownloadServiceProperty =
                 DependencyProperty.Register("DownloadService", typeof(object),
               typeof(DownloadInfoDisplay), new UIPropertyMetadata(null));

        public static readonly DependencyProperty MaxDisplayedValuesProperty =
                 DependencyProperty.Register("MaxDisplayedValues", typeof(object),
               typeof(DownloadInfoDisplay), new UIPropertyMetadata(0));

        public DownloadService DownloadService
        {
            get => (DownloadService)GetValue(DownloadServiceProperty);
            set {
                ((DownloadService)GetValue(DownloadServiceProperty)).DownloadProgressChanged -= OnDownloadProgressChanged;
                SetValue(DownloadServiceProperty, value);
                value.DownloadProgressChanged += OnDownloadProgressChanged;
            }
        }

        public int MaxDisplayedValues 
        {
            get => Application.Current.Dispatcher.Invoke(() => (int)GetValue(MaxDisplayedValuesProperty));
            set => Application.Current.Dispatcher.Invoke(() => SetValue(MaxDisplayedValuesProperty, value)); 
        }
        #endregion

        public PathGeometry? PathGeometry
        {
            get => _pathGeometry;
            set => SetProperty(ref _pathGeometry, value);
        }
        private PathGeometry? _pathGeometry;

        public double DownloadSpeedPointOriginX
        {
            get => _downloadSpeedPointOriginX;
            set => SetProperty(ref _downloadSpeedPointOriginX, value);
        }
        private double _downloadSpeedPointOriginX;

        public double DownloadSpeedPointOriginY
        {
            get => _downloadSpeedPointOriginY;
            set => SetProperty(ref _downloadSpeedPointOriginY, value);
        }
        private double _downloadSpeedPointOriginY;

        private Queue<double> DiscreteSpeedValues;

        double MaxValueSoFar;

        private Stopwatch stopWatch;

        private float TopSpaceFactor = 1.3f;

        public DownloadInfoDisplay()
        {
            InitializeComponent();
            DiscreteSpeedValues = new();
            DownloadSpeedVisualization.DataContext = this;
            DownloadSpeedText.DataContext = this;

            stopWatch = new Stopwatch();
            stopWatch.Start();
        }

        private void Redraw()
        {
            Application.Current.Dispatcher.Invoke(() => 
            {
                PathGeometry = ComputeCurve();
            });
        }

        
        public void OnDownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e)
        {
            if (stopWatch.ElapsedMilliseconds < 1000)
                return;

            stopWatch.Restart();
            DiscreteSpeedValues.Enqueue(e.BytesPerSecondSpeed);
            if (DiscreteSpeedValues.Count() >= MaxDisplayedValues)
            {
                DiscreteSpeedValues.Dequeue();
            }

            if (e.BytesPerSecondSpeed * TopSpaceFactor > MaxValueSoFar)
            {
                MaxValueSoFar = e.BytesPerSecondSpeed * TopSpaceFactor;
            }
            Redraw();
        }

        private PathGeometry? ComputeCurve()
        {
            if (DiscreteSpeedValues.Count() == 0) return null; 
            var height = RenderSize.Height;
            var width = RenderSize.Width;
            var stepping = width * 2/3 / MaxDisplayedValues;

            IEnumerable<double> InterpolatedSpeedValues = DiscreteSpeedValues.Select(x => CurveHelper.InterpolateX(MaxValueSoFar, height, x));
            var startPoint = new Point(0, InterpolatedSpeedValues.ElementAt(0));

            PointCollection points = new PointCollection();
            for (int i = 1; i < DiscreteSpeedValues.Count(); i++)
            {
                var x0 = InterpolatedSpeedValues.ElementAt(i - 1);
                var x1 = InterpolatedSpeedValues.ElementAt(i);

                points.Add(new Point(i * stepping - (stepping * 2/3), x0));
                points.Add(new Point(i * stepping - (stepping * 1/3), x1));
                points.Add(new Point(i * stepping, x1));
            }

            DownloadSpeedPointOriginY = InterpolatedSpeedValues.Last() - DownloadSpeedText.Height/2;
            DownloadSpeedPointOriginX = (DiscreteSpeedValues.Count()-1) * stepping;

            return CurveHelper.ConstructPolyBezier(startPoint, points);
        }


        #region INotifyPropertyChangedMembers
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            property = value;
            OnPropertyChanged(propertyName);
        }
        #endregion

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Redraw();
        }
    }
}
