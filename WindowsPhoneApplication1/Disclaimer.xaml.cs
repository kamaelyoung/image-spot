using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace ImageSpot
{
    public partial class Disclaimer : PhoneApplicationPage
    {
        public Disclaimer()
        {
            InitializeComponent();
            webBrowser1.NavigateToString(@"
                <html>
                    <head>
                        <title>Privacy</title>
                    </head>
                    <body>
                        <b>Use of Location</b>
                        <p>
                            ImageSpot uses your current location to retrieve photos from flickr, that have been taken in your area. Therefor your location is sent to flickr. No other user informations will be uploaded or linked with your location. For more information see <a href='http://www.flickr.com/help/privacy/'>http://www.flickr.com/help/privacy/</a>.
                        </p>
                    </body>
                </html>");
        }
    }
}