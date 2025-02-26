using Microsoft.Maui.Controls;
using FotoApp.MVVM.Model;
using FotoApp.MVVM.ViewModel;
using FotoApp.MVVM.Data;

namespace FotoApp.MVVM.View
{
    public partial class UserPhotos : ContentPage
    {
        public UserPhotos()
        {
            InitializeComponent();
            BindingContext = new UserPhotosViewModel(); // Geen database-parameter meer nodig
        }
    }
}
