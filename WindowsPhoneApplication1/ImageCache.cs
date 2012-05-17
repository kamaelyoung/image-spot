using System;
using System.Collections.Generic;
using ImageSpot.ViewModels;

namespace ImageSpot
{
    public class ImageCache
    {
        private Dictionary<String, ImageViewModel> _cache;
        private static ImageCache _instance;

        private ImageCache()
        {
            _cache = new Dictionary<string, ImageViewModel>();
        }

        public static ImageCache GetInstance()
        {
            if(_instance == null)
                _instance = new ImageCache();
            return _instance;
        }

        public void Add(ImageViewModel img)
        {
            if (_cache.ContainsKey(img.Id))
                _cache[img.Id] = img;
            else
                _cache.Add(img.Id, img);   
        }

        public ImageViewModel Get(String id)
        {
            if (_cache.ContainsKey(id))
                return _cache[id];
            else
                return null;
        }

        public ImageViewModel RandomImage()
        {
            if (_cache.Count == 0)
                return null;
            Random rnd = new Random();
            int id = rnd.Next(0, _cache.Count);
            var arr = new ImageViewModel[_cache.Count];
            _cache.Values.CopyTo(arr, 0);
            return arr[id];
        }

        public String Random()
        {
            if (_cache.Count == 0)
                return "";
            Random rnd = new Random();
            int id = rnd.Next(0, _cache.Count);
            var arr = new ImageViewModel[_cache.Count];
            _cache.Values.CopyTo(arr, 0);
            return arr[id].Id;
        }
    }
}
