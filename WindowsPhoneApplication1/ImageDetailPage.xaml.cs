using System.Diagnostics;
using System.Windows;
using ImageSpot.ViewModels;
using Microsoft.Phone.Controls;

namespace ImageSpot
{
    public partial class ImageDetailPage : PhoneApplicationPage
    {
        public ImageDetailPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("id"))
            {
                DataContext = ImageCache.GetInstance().Get(NavigationContext.QueryString["id"]);
                DetailGrid.DataContext = ImageCache.GetInstance().Get(NavigationContext.QueryString["id"]);
                imageMap.DataContext = ImageCache.GetInstance().Get(NavigationContext.QueryString["id"]);
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