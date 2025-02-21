using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace FotoApp.MVVM.ViewModel
{
    public class HomePageViewModel
    {
        public ICommand NavigateCommand { get; }

        public HomePageViewModel()
        {
            NavigateCommand = new Command<string>(Navigate);
        }

        private async void Navigate(string pageName)
        {
            // Switch voor verschillende pagina's
            Page page = pageName switch
            {
				"HomePage" => new FotoApp.MVVM.View.HomePage(),
                "Assignments" => new FotoApp.MVVM.View.Assignments(),
                "User" => new FotoApp.MVVM.View.UserManagement(),
                _ => null
			};

            if (page != null)
            {
                // Zorg dat er een NavigationPage is ingesteld in App.xaml.cs
                await Application.Current.MainPage.Navigation.PushAsync(page);
            }
        }
    }
}
