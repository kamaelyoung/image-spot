using System;
using System.ComponentModel;
using System.Device.Location;
using Microsoft.Devices.Sensors

namespace ImageSpot.ViewModels
{
    public class DistanceViewModel : INotifyPropertyChanged
    {
        private GeoCoordinateWatcher _watcher;
        Compass c;
        GeoCoordinate target;
        GeoCoordinate lastKnownPosition;
            

        #region "properties"
        private double _distance;

        public double Distance
        {
            get { return _distance; }
            set { 
                _distance = value;
                Notify("Distance");
            }
        }

        private String _unit;

        public String DistanceUnit
        {
            get { return _unit; }
            set { 
                _unit = value;
                Notify("DistanceUnit");
            }
        }

        private double _degree;

        public double Degree
        {
            get { return _degree; }
            set { 
                _degree = value;
                Notify("Degree";
            }
        }

        private void Notify(String name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

#endregion        
                
        public DistanceViewModel(GeoCoordinate target)
        {
            this.target = target;
            lastKnownPosition = target;
            Distance = -1;
            DistanceUnit = "???";
            Degree = 0;
            //start GPS
            _watcher = new GeoCoordinateWatcher() { MovementThreshold = 10};
            c = new Compass();
            _watcher.StatusChanged += delegate(Object sender, GeoPositionStatusChangedEventArgs a)
            {
                if (a.Status == GeoPositionStatus.Ready)
                    CalcDistance();
            };
            
            c.CurrentValueChanged +=delegate(object sender, SensorReadingEventArgs<CompassReading> e) {
                CalcCourse();
            };

            c.Start();
            _watcher.Start();
        }

        private void CalcCourse()
        {
            
        }

        private void CalcDistance() {
            double dist = _watcher.Position.Location.GetDistanceTo(target);
            if(dist > 1000) {
                DistanceUnit = "km";
                dist = Math.Round(dist / 1000, 2);
            } else {
                DistanceUnit = "m";
                dist = dist;
            }
        }
    }
}
