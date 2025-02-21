using SQLiteBrowser;
using FotoApp.MVVM.View;

namespace FotoApp
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OpenDatabaseBrowser(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DatabaseBrowserPage(Path.Combine(FileSystem.AppDataDirectory, "FotoApp.db")));
        }

        private async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            bool confirmLogout = await DisplayAlert("Uitloggen", "Weet u zeker dat u wilt uitloggen?", "Ja", "Nee");
            if (!confirmLogout) return;
            try
            {
                await App.Database.SetActiveUser(0);
                App.CurrentUser = null;
                await Navigation.PushAsync(new StartPage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", $"Er is iets misgegaan tijdens het uitloggen: {ex.Message}", "OK");
            }
        }
        private async void OnManageUsersClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ManageUser());
        }
    }

}
