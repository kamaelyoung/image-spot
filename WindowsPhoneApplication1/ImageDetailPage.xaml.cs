using System;
using System.Device.Location;
using System.Diagnostics;
using System.Windows;
using ImageSpot.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using System.IO.IsolatedStorage;

namespace ImageSpot
{
    public partial class ImageDetailPage : PhoneApplicationPage
    {
        ImageViewModel item;
        private GeoCoordinateWatcher _watcher;
        Compass compass;
        private double _lastCompass;
        GeoCoordinate currentCoordinate;
        IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;

        public ImageDetailPage()
        {
            InitializeComponent();
        }



        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("id"))
            {
                item = ImageCache.GetInstance().Get(NavigationContext.QueryString["id"]);
                GoLive();
                DataContext = item;
                DetailGrid.DataContext = item;
                imageMap.DataContext = item;
                AutoFocusMap();
            }
        }

        public void GoLive()
        {
            if (Compass.IsSupported)
            {
                compass = new Compass();
                compass.TimeBetweenUpdates = TimeSpan.FromMilliseconds(10);
                compass.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<CompassReading>>(compass_CurrentValueChanged);
                compass.Start();
            }
            else
            {
                noDir.Text = "compass not supported by your device!";
            }

            if (appSettings.Contains("allowGps") && appSettings["allowGps"] as Boolean? == true)
            {
                _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default) { MovementThreshold = 500 };
                _watcher.PositionChanged += delegate(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
                {
                    Dispatcher.BeginInvoke(() => RecalcPosition());
                };
                _watcher.Start(false);
            }
        }

        void compass_CurrentValueChanged(object sender, SensorReadingEventArgs<CompassReading> e)
        {
            Dispatcher.BeginInvoke(() => UpdateUI(e.SensorReading));
        }

        private void RecalcPosition()
        {
            if (_watcher == null || _watcher.Status != GeoPositionStatus.Ready || item.Position == null) return;
            double distance = item.Position.GetDistanceTo(_watcher.Position.Location);
            if (distance > 1000)
            {
                txtDistanceUnit.Text = "km";
                txtDistance.Text = (Math.Round(distance / 1000.0, 2)).ToString();
            }
            else
            {
                txtDistanceUnit.Text = "m";
                txtDistance.Text = Math.Round(distance, 2).ToString();
            }
        }

        private void UpdateUI(CompassReading compassReading)
        {
            if (item.Position != null && _watcher != null)
            {
                this.currentCoordinate = _watcher.Position.Location;
                double y = item.Position.Latitude - currentCoordinate.Latitude;
                double x = item.Position.Longitude - currentCoordinate.Longitude;
                double phi = Math.Atan2(y, x) * 100 / 1.7222; //i don't know why, but it is working!
                float delta = (float)(compassReading.MagneticHeading + phi - 90);
                magneticLine.X2 = magneticLine.X1 - (200 * Math.Sin(MathHelper.ToRadians(delta)));
                magneticLine.Y2 = magneticLine.Y1 - (200 * Math.Cos(MathHelper.ToRadians(delta)));
                magneticLine.Visibility = System.Windows.Visibility.Visible; ;
                noDir.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        


        private void AutoFocusMap()
        {
            if(DataContext is ImageViewModel)
                imageMap.SetView((DataContext as ImageViewModel).Position, 10);
        }      
    }
}