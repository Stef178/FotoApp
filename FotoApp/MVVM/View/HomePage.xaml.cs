namespace FotoApp.MVVM.View
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
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

        private async void OnViewOtherPhotosClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CommentsPage());
        }
    }
}
