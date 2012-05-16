using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Devices.Sensors;

namespace ImageSpot
{
    //src of this file is from http://mark.mymonster.nl/2010/10/24/shake-that-windows-phone-7-and-detect-it/
    public class AccelerometerSensorWithShakeDetection : IDisposable
    {
        private const double ShakeThreshold = 0.7;
        private readonly Accelerometer _sensor = new Accelerometer();
        private AccelerometerReadingEventArgs _lastReading;
        private int _shakeCount;
        private bool _shaking;

        public AccelerometerSensorWithShakeDetection()
        {
            var sensor = new Accelerometer();
            if (sensor.State == SensorState.NotSupported)
                throw new NotSupportedException("Accelerometer not supported on this device");
            _sensor = sensor;
        }

        public SensorState State
        {
            get { return _sensor.State; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_sensor != null)
                _sensor.Dispose();
        }

        #endregion

        private event EventHandler ShakeDetectedHandler;

        public event EventHandler ShakeDetected
        {
            add
            {
                ShakeDetectedHandler += value;
                _sensor.ReadingChanged += ReadingChanged;
            }
            remove
            {
                ShakeDetectedHandler -= value;
                _sensor.ReadingChanged -= ReadingChanged;
            }
        }

        public void Start()
        {
            if (_sensor != null)
                _sensor.Start();
        }

        public void Stop()
        {
            if (_sensor != null)
                _sensor.Stop();
        }

        private void ReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            //Code for checking shake detection
            if (_sensor.State == SensorState.Ready)
            {
                AccelerometerReadingEventArgs reading = e;
                try
                {
                    if (_lastReading != null)
                    {
                        if (!_shaking && CheckForShake(_lastReading, reading, ShakeThreshold) && _shakeCount >= 1)
                        {
                            //We are shaking
                            _shaking = true;
                            _shakeCount = 0;
                            OnShakeDetected();
                        }
                        else if (CheckForShake(_lastReading, reading, ShakeThreshold))
                        {
                            _shakeCount++;
                        }
                        else if (!CheckForShake(_lastReading, reading, 0.2))
                        {
                            _shakeCount = 0;
                            _shaking = false;
                        }
                    }
                    _lastReading = reading;
                }
                catch(Exception ex)
                {
                    /* ignore errors */
                }
            }
        }

        private void OnShakeDetected()
        {
            if (ShakeDetectedHandler != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    ShakeDetectedHandler.BeginInvoke(this, EventArgs.Empty, EndAsyncEvent, null);
                });
                
            }
        }

        private void EndAsyncEvent(IAsyncResult result)
        {
            ((EventHandler<EventArgs>)result.AsyncState).EndInvoke(result);

        }


        private static bool CheckForShake(AccelerometerReadingEventArgs last, AccelerometerReadingEventArgs current,
                                            double threshold)
        {
            double deltaX = Math.Abs((last.X - current.X));
            double deltaY = Math.Abs((last.Y - current.Y));
            double deltaZ = Math.Abs((last.Z - current.Z));

            return (deltaX > threshold && deltaY > threshold) ||
                    (deltaX > threshold && deltaZ > threshold) ||
                    (deltaY > threshold && deltaZ > threshold);
        }
    }
}
