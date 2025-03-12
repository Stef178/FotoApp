using Microsoft.Maui.Controls;
using FotoApp.MVVM.ViewModel;

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
