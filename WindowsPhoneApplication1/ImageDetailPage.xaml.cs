using System.Device.Location;
using System.Diagnostics;
using System.Windows;
using ImageSpot.ViewModels;
using Microsoft.Phone.Controls;


namespace ImageSpot
{
    public partial class ImageDetailPage : PhoneApplicationPage
    {
        ImageViewModel item;
        public ImageDetailPage()
        {
            InitializeComponent();
        }


        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("id"))
            {
                item = ImageCache.GetInstance().Get(NavigationContext.QueryString["id"]);
                DataContext = item;
                DetailGrid.DataContext = item;
                imageMap.DataContext = item;
                AutoFocusMap();
            }
        }

        private void AutoFocusMap()
        {
            if(DataContext is ImageViewModel)
                imageMap.SetView((DataContext as ImageViewModel).Position, 10);
        }
       
    }
}