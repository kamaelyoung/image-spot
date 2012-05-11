using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Device.Location;
using System.Diagnostics;

namespace ImageSpot.ViewModels
{
    public class ImageListViewModel : ObservableCollection<ImageViewModel>
    {
        private readonly ObservableCollection<GeoCoordinate> _pushpinCollection;
        private readonly ImageCache _cache;

        public ImageListViewModel() : this(ImageCache.GetInstance()){}

        private ImageListViewModel(ImageCache cache)
        {
            _pushpinCollection = new ObservableCollection<GeoCoordinate>();
            _cache = cache;
        }

        public void AddImage(ImageViewModel img)
        {
            Debug.WriteLine("added image");
            if (!Contains(img))
                this.Add(img);

            if(_cache != null)
                _cache.Add(img);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


    }
}
