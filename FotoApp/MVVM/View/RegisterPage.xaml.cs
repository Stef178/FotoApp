using System;
using FotoApp.MVVM.Model;

namespace FotoApp.MVVM.View
{
	public partial class RegisterPage : ContentPage
	{
		public RegisterPage()
		{
			InitializeComponent();
		}

		private async void OnAccountAanmakenClicked(object sender, EventArgs e)
		{
			string name = UsernameEntry.Text?.Trim();
			string email = EmailEntry.Text?.Trim();
			string password = PasswordEntry.Text;

			if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
			{
				await DisplayAlert("Fout", "Alle velden moeten worden ingevuld.", "OK");
				return;
			}

			var participant = new User
			{
				Username = name,
				Email = email,
				Password = password
			};

			try
			{
				await App.Database.AddAsync(participant);

				await DisplayAlert("Succes", "Account aangemaakt!", "OK");

				await Navigation.PushAsync(new LoginPage());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error saving participant: {ex.Message}");
				await DisplayAlert("Fout", "Er is iets misgegaan bij het aanmaken van het account.", "OK");
			}
		}
	}
}
