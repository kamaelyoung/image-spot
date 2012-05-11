using System;
using System.Device.Location;
using System.Linq;
using System.Windows;
using ImageSpot.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System.Collections.Generic;

namespace ImageSpot
{
    public partial class MainPage : PhoneApplicationPage
    {
        private List<GeoCoordinate> coordinates;
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
            FlickrWrapper.GetInstance().GetPhotos(delegate(ImageViewModel img) { model.AddImage(img); coordinates.Add(img.Position); AutoFocusMap();});
            LayoutRoot.DataContext = model;
            pictureMap.Mode = new RoadMode();
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
    }
}