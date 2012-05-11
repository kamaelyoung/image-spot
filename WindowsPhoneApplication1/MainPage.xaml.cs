using System.Windows;
using ImageSpot.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;

namespace ImageSpot
{
    public partial class MainPage : PhoneApplicationPage
    {
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
            var model = new ImageListViewModel();
            FlickrWrapper.GetInstance().GetPhotos(model.AddImage);
            LayoutRoot.DataContext = model;
            pictureMap.Mode = new RoadMode();
        }
    }
}