using System;
using System.Device.Location;
using System.Linq;
using System.Windows;
using ImageSpot.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System.Collections.Generic;
using ShakeGestures;

namespace ImageSpot
{
    public partial class MainPage : PhoneApplicationPage
    {
        private List<GeoCoordinate> coordinates;
        private AccelerometerSensorWithShakeDetection shake;


        // Konstruktor
        public MainPage()
        {
            InitializeComponent();

            // Beispieldaten als Datenkontexts des Listbox-Steuerelements festlegen
            this.Loaded +=new RoutedEventHandler(MainPage_Loaded);
        }

        // Daten für die ViewModel-Elemente laden
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            coordinates = new List<GeoCoordinate>();
            var model = new ImageListViewModel();
            FlickrWrapper.GetInstance().AddObserver(delegate(ImageViewModel img) { model.AddImage(img); coordinates.Add(img.Position); AutoFocusMap(); });
            //FlickrWrapper.GetInstance().GetPhotos();
            LayoutRoot.DataContext = model;
            pictureMap.Mode = new RoadMode();

            // register shake event

            ShakeGesturesHelper.Instance.ShakeGesture += new EventHandler<ShakeGestureEventArgs>(shake_ShakeDetected);
            ShakeGesturesHelper.Instance.MinimumRequiredMovesForShake = 3;
            ShakeGesturesHelper.Instance.Active = true;
        }

        private void AutoFocusMap()
        {
            var bounds = new LocationRect(
                coordinates.Max((p) => p.Latitude),
                coordinates.Min((p) => p.Longitude),
                coordinates.Min((p) => p.Latitude),
                coordinates.Max((p) => p.Longitude));
            pictureMap.SetView(bounds);
        }

        private void ImageList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(ImageList.SelectedItem is ImageViewModel)
            {
                NavigationService.Navigate(
                    new Uri("/ImageDetailPage.xaml?id=" + (ImageList.SelectedItem as ImageViewModel).Id, UriKind.Relative));
            }
        }

        void shake_ShakeDetected(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(() => {
                var cache = ImageCache.GetInstance();
                var id = cache.Random();
                if (!String.IsNullOrWhiteSpace(id))
                    ((App)Application.Current).RootFrame.Navigate(new Uri("/ImageDetailPage.xaml?id=" + id, UriKind.Relative));
            });
        }
    }
}