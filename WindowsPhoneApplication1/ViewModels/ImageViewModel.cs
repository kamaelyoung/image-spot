using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Device.Location;
using Microsoft.Devices.Sensors;
using System.Windows.Threading;

namespace ImageSpot.ViewModels
{
    public class ImageViewModel : INotifyPropertyChanged
    {
        private string _name;
        public String Name
        {
            get { return _name; }
            set { _name = value;
                Notify("Name");
            }
        }

        private Uri _imageUri;
        public Uri ImageUri
        {
            get { return _imageUri; }
            set { _imageUri = value;
                Notify("ImageUri");
            }
        }

        private BitmapImage _image;
        public BitmapImage Image
        {
            get { return _image; }
            set { _image = value;
            Notify("Image");
            }
        }

        private GeoCoordinate _position;
        public GeoCoordinate Position
        {
            get { return _position; }
            set { _position = value;
            Notify("Position");
            }
        }

        private DateTime _takenOn;
        public DateTime TakenOn
        {
            get { return _takenOn; }
            set { _takenOn = value;
            Notify("TakenOn");
            }
        }

        private Uri _authorUri;
        public Uri AuthorUri
        {
            get { return _authorUri; }
            set { _authorUri = value;
            Notify("AuthorUri");
            }
        }

        private string _authorName;
        public String AuthorName
        {
            get { return _authorName; }
            set { _authorName = value;
            Notify("AuthorName");
            }
        }

        private string _id;
        public String Id
        {
            get { return _id; }
            set { _id = value;
            Notify("Id");
            }
        }

        private string _description;
        public String Description
        {
            get { return _description; }
            set { _description = value;
            Notify("Description");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(String propertyName)
        {
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        

        public override bool Equals(object obj)
        {
            if ((obj is ImageViewModel))
                return (obj as ImageViewModel).Id == Id;
            return false;
        }
    }
}
