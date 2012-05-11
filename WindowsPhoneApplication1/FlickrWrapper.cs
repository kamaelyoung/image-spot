using System;
using System.Device.Location;
using System.Windows.Media.Imaging;
using FlickrNet;
using ImageSpot.ViewModels;

namespace ImageSpot
{
    public class FlickrWrapper
    {
        private const string Key = "9b23a7f748aadf005f5bd023fadcd879";
        private const string Secret = "f2e35e13b2b59d60";
        private const string OwnerPrefix = @"http://www.flickr.com/people/";
        private static FlickrWrapper _instance;
        private readonly Flickr _flickr;
        private GeoCoordinateWatcher _watcher;

        private static readonly PhotoSearchOptions DefaultOptions = new PhotoSearchOptions
                                                                        {
                                                                            ContentType = ContentTypeSearch.PhotosOnly,
                                                                            HasGeo = true,
                                                                            PerPage = 24,
                                                                            SortOrder =
                                                                                PhotoSearchSortOrder.DatePostedDescending,
                                                                            Extras =
                                                                                PhotoSearchExtras.Geo |
                                                                                PhotoSearchExtras.AllUrls |
                                                                                PhotoSearchExtras.DateTaken |
                                                                                PhotoSearchExtras.Description |
                                                                                PhotoSearchExtras.OwnerName
                                                                        };


        public delegate void Callback(ImageViewModel img);

        private FlickrWrapper()
        {
            _flickr = new Flickr(Key, Secret);
            _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            _watcher.Start(true);
        }

        public static FlickrWrapper GetInstance()
        {
            if (_instance == null)
                _instance = new FlickrWrapper();
            return _instance;
        }

        public void GetPhotos(Callback c)
        {
            var options = DefaultOptions;
            if (_watcher.Status == GeoPositionStatus.Ready)
            {
                options.RadiusUnits = RadiusUnit.Kilometers;
                options.Radius = 10;
                options.Longitude = _watcher.Position.Location.Longitude;
                options.Latitude = _watcher.Position.Location.Latitude;
            }
            _flickr.PhotosSearchAsync(options, delegate(FlickrResult<PhotoCollection> result)
                                                         {
                                                             if (result.HasError) return;
                                                             foreach (var curr in result.Result)
                                                             {
                                                                 c(new ImageViewModel
                                                                       {
                                                                           AuthorUri =
                                                                               new Uri(OwnerPrefix + curr.OwnerName),
                                                                           Name = curr.Title,
                                                                           Image = new BitmapImage(new Uri(curr.LargeUrl)),
                                                                           ImageUri = new Uri(curr.LargeUrl),
                                                                           Position =
                                                                               new GeoCoordinate(curr.Latitude,
                                                                                                 curr.Longitude),
                                                                           TakenOn = curr.DateTaken,
                                                                           Id = curr.PhotoId,
                                                                           AuthorName = curr.OwnerName,
                                                                           Description = curr.Description
                                                                       });
                                                             }
                                                         });
        }
    }
}
