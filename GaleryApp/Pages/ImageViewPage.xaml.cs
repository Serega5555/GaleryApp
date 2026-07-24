using GaleryApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GaleryApp.Pages
{
    public partial class ImageViewPage : ContentPage
    {
        private ImageModel imageModel;
        public ImageViewPage(ImageModel model)
        {
            InitializeComponent();
            imageModel = model;
            BindingContext = imageModel;
            imageView.Source = imageModel.FilePath;
            dateLabel.Text = $"Дата съемки: {imageModel.DateTaken}";
        }
    }
}
