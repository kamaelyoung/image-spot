using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Device.Location;

namespace ImageSpot.ViewModels
{
    public class ImageViewModel : INotifyPropertyChanged
    {
        public String Name { get; set; }
        public Uri ImageUri { get; set; }
        public BitmapImage Image { get; set; }
        public GeoCoordinate Position { get; set; }
        public DateTime TakenOn { get; set; }
        public Uri AuthorUri { get; set; }
        public String Id { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
