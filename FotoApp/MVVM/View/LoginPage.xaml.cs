using FotoApp.MVVM.Data;
using FotoApp.MVVM.Model;

namespace FotoApp.MVVM.View;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim();
        string password = PasswordEntry.Text?.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Fout", "Vul alstublieft zowel e-mailadres als wachtwoord in.", "OK");
            return;
        }

        try
        {
            if (email == App.AdminUser.Email && password == App.AdminUser.Password)
            {
                App.CurrentUser = App.AdminUser; 
                await DisplayAlert("Succes", "Admin succesvol ingelogd!", "OK");

                Application.Current.MainPage = new NavigationPage(new MainPage());
                return;
            }

            var users = await App.Database.GetAllAsync<User>();
            var matchingUser = users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (matchingUser != null)
            {
                await App.Database.SetActiveUser(matchingUser.Id);
                App.CurrentUser = matchingUser;
                await DisplayAlert("Succes", "U bent succesvol ingelogd!", "OK");

                Application.Current.MainPage = new NavigationPage(new HomePage());
            }
            else
            {
                await DisplayAlert("Fout", "Onjuist e-mailadres of wachtwoord. Probeer het opnieuw.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Er is iets misgegaan: {ex.Message}", "OK");
        }
    }

}
