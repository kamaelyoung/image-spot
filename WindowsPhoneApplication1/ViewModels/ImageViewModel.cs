using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Device.Location;
using Microsoft.Devices.Sensors;
using System.Windows.Threading;

namespace ImageSpot.ViewModels
{
    public class ImageViewModel : INotifyPropertyChanged, IDisposable
    {
        private GeoCoordinateWatcher _watcher;
        Compass compass;
        private Dispatcher _dispatcher;
        private double _lastCompass;

        public void GoLive(Dispatcher d)
        {
            _dispatcher = d;
            compass = new Compass();
            compass.TimeBetweenUpdates = new TimeSpan(0, 0, 0, 0, 500);
            compass.CurrentValueChanged += delegate(object sender, SensorReadingEventArgs<CompassReading> e)
            {
                RecalcBearing();
            };
            _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default) { MovementThreshold = 500 };
            _watcher.PositionChanged += delegate(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
            {
                this.Position = Position;
                RecalcPosition();
            };


            _watcher.StatusChanged += delegate(Object sender, GeoPositionStatusChangedEventArgs a)
            {
                if (a.Status == GeoPositionStatus.Ready)
                    RecalcPosition();
            };

            _watcher.Start(false);
            compass.Start();
        }

        private string _name;
        public String Name
        {
            get { return _name; }
            set { _name = value;
                Notify("Name");
            }
        }

        private Uri _imageUri;
        public Uri ImageUri
        {
            get { return _imageUri; }
            set { _imageUri = value;
                Notify("ImageUri");
            }
        }

        private BitmapImage _image;
        public BitmapImage Image
        {
            get { return _image; }
            set { _image = value;
            Notify("Image");
            }
        }

        private GeoCoordinate _position;
        public GeoCoordinate Position
        {
            get { return _position; }
            set { _position = value;
            Notify("Position");
            }
        }

        private DateTime _takenOn;
        public DateTime TakenOn
        {
            get { return _takenOn; }
            set { _takenOn = value;
            Notify("TakenOn");
            }
        }

        private Uri _authorUri;
        public Uri AuthorUri
        {
            get { return _authorUri; }
            set { _authorUri = value;
            Notify("AuthorUri");
            }
        }

        private string _authorName;
        public String AuthorName
        {
            get { return _authorName; }
            set { _authorName = value;
            Notify("AuthorName");
            }
        }

        private string _id;
        public String Id
        {
            get { return _id; }
            set { _id = value;
            Notify("Id");
            }
        }

        private string _description;
        public String Description
        {
            get { return _description; }
            set { _description = value;
            Notify("Description");
            }
        }

        private string _distanceUnit;
        public String DistanceUnit
        {
            get { return _distanceUnit; }
        }

        private double _bearing;
        public double Bearing
        {
            get { return _bearing; }
        }

        private double _distance;
        public double Distance
        {
            get { return _distance; }
        }

        private void Notify(String propertyName)
        {
            if (PropertyChanged != null)
            {
                if (_dispatcher.CheckAccess())
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
                else
                {
                    _dispatcher.BeginInvoke(PropertyChanged, this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public override bool Equals(object obj)
        {
            if ((obj is ImageViewModel))
                return (obj as ImageViewModel).Id == Id;
            return false;
        }

        private void RecalcPosition()
        {
            if (_watcher == null || _watcher.Status != GeoPositionStatus.Ready || Position == null) return;
            double distance = Position.GetDistanceTo(_watcher.Position.Location);
            if (distance > 1000)
            {
                _distanceUnit = "km";
                _distance = Math.Round(distance / 1000.0, 2);
            }
            else
            {
                _distanceUnit = "m";
                _distance = Math.Round(distance, 2);
            }
            Notify("DistanceUnit");
            Notify("Distance");
            RecalcBearing();
        }

        public void RecalcBearing()
        {
            if (_watcher == null || compass == null || _watcher.Status != GeoPositionStatus.Ready || Position == null || !compass.IsDataValid) return;
            var p1 = Position;
            var p2 = _watcher.Position.Location;
            double deltaLon = Math.Abs(p1.Longitude - p2.Longitude);
            double deltaLat = Math.Abs(p1.Latitude - p2.Latitude);
            double bearing = Math.Atan2(Math.Sin(deltaLon) * Math.Cos(p2.Latitude), Math.Cos(p1.Latitude) * Math.Sin(p2.Latitude) - Math.Sin(p1.Latitude) * Math.Cos(p2.Latitude) * Math.Cos(deltaLon));
            _bearing = Math.Round(bearing - compass.CurrentValue.TrueHeading, 3);
            Notify("Bearing");
        }

        public void Dispose()
        {
            _watcher.Stop();
            _watcher.Dispose();
            compass.Stop();
            compass.Dispose();
        }
    }
}
