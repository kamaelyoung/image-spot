using System;
using System.Device.Location;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using FlickrNet;
using ImageSpot.ViewModels;
using System.IO.IsolatedStorage;

namespace ImageSpot
{
    public class FlickrWrapper
    {
        private const string Key = "9b23a7f748aadf005f5bd023fadcd879";
        IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
        private const string Secret = "f2e35e13b2b59d60";
        private const string OwnerPrefix = @"http://www.flickr.com/people/";
        private static FlickrWrapper _instance;
        private readonly Flickr _flickr;
        private GeoCoordinateWatcher _watcher;
        private List<Callback> observers;
        private static readonly PhotoSearchOptions DefaultOptions = new PhotoSearchOptions
                                                                        {
                                                                            ContentType = ContentTypeSearch.PhotosOnly,
                                                                            HasGeo = true,
                                                                            PerPage = 24,
                                                                            Accuracy = FlickrNet.GeoAccuracy.City,
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
            observers = new List<Callback>();
            _flickr = new Flickr(Key, Secret);

            if (!appSettings.Contains("allowGps") || appSettings["allowGps"] as Boolean? == false)
            {
                GetPhotos();
            }
            else
            {
                _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default) { MovementThreshold = 500 };
                _watcher.Start(false);
                _watcher.StatusChanged += delegate(Object sender, GeoPositionStatusChangedEventArgs a)
                {
                    if (a.Status == GeoPositionStatus.Ready)
                        GetPhotos();
                };
            }
        }

        public static FlickrWrapper GetInstance()
        {
            if (_instance == null)
                _instance = new FlickrWrapper();
            return _instance;
        }

        public void AddObserver(Callback c)
        {
            if (!observers.Contains(c))
                observers.Add(c);
        }

        public void GetPhotos()
        {
            var options = DefaultOptions;
            if (_watcher != null && _watcher.Status == GeoPositionStatus.Ready)
            {
                options.BoundaryBox = new BoundaryBox(_watcher.Position.Location.Longitude- 0.5, _watcher.Position.Location.Latitude - 0.5, _watcher.Position.Location.Longitude + 0.5, _watcher.Position.Location.Latitude + 0.5, GeoAccuracy.City);
                //options.Longitude = _watcher.Position.Location.Longitude;
                //options.Latitude = _watcher.Position.Location.Latitude;
            }
            _flickr.PhotosSearchAsync(options, delegate(FlickrResult<PhotoCollection> result)
                                                         {
                                                             if (result.HasError) return;
                                                             foreach (var curr in result.Result)
                                                             {
                                                                 var model = new ImageViewModel
                                                                       {
                                                                           AuthorUri =
                                                                               new Uri(OwnerPrefix + curr.OwnerName),
                                                                           Name = curr.Title,
                                                                           Image = new BitmapImage(new Uri(curr.Small320Url)),
                                                                           ImageUri = new Uri(curr.Small320Url),
                                                                           Position =
                                                                               new GeoCoordinate(curr.Latitude,
                                                                                                 curr.Longitude),
                                                                           TakenOn = curr.DateTaken,
                                                                           Id = curr.PhotoId,
                                                                           AuthorName = curr.OwnerName,
                                                                           Description = curr.Description
                                                                       };
                                                                 foreach (var item in observers)
                                                                 {
                                                                     item(model);
                                                                 }
                                                             }
                                                         });
        }
    }
}
