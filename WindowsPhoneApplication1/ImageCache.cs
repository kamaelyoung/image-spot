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
    }
}
