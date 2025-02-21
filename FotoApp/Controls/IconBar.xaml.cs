using Microsoft.Maui.Controls;

namespace FotoApp.Controls;

public partial class IconBar : ContentView
{
    public IconBar()
    {
        InitializeComponent();

        BindingContext = new FotoApp.MVVM.ViewModel.HomePageViewModel();
    }
}
