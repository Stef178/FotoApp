using FotoApp.MVVM.Data;
using FotoApp.MVVM.Model;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FotoApp.MVVM.View
{
    public partial class ManageUser : ContentPage
    {
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

        public ManageUser()
        {
            InitializeComponent();
            LoadUsers();
            BindingContext = this;
        }

        private async void LoadUsers()
        {
            var users = await App.Database.GetAllAsync<User>();
            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }

        private async void OnAddUser(object sender, EventArgs e)
        {
            string username = await DisplayPromptAsync("Nieuwe gebruiker", "Gebruikersnaam:");
            string email = await DisplayPromptAsync("Nieuwe gebruiker", "E-mail:");
            string password = await DisplayPromptAsync("Nieuwe gebruiker", "Wachtwoord:");

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Fout", "Alle velden zijn verplicht!", "OK");
                return;
            }

            var newUser = new User { Username = username, Email = email, Password = password, IsActive = false };
            await App.Database.AddAsync(newUser);
            LoadUsers();
        }

        private async void OnDeleteUser(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int userId)
            {
                bool confirm = await DisplayAlert("Bevestigen", "Weet je zeker dat je deze gebruiker wilt verwijderen?", "Ja", "Nee");
                if (!confirm) return;

                var user = await App.Database.GetAsync<User>(userId);
                if (user != null)
                {
                    await App.Database.DeleteAsync(user);
                    LoadUsers();
                }
            }
        }

        private async void OnEditUser(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int userId)
            {
                var user = await App.Database.GetAsync<User>(userId);
                if (user == null) return;

                string newUsername = await DisplayPromptAsync("Gebruiker bewerken", "Nieuwe gebruikersnaam:", initialValue: user.Username);
                if (!string.IsNullOrWhiteSpace(newUsername))
                {
                    user.Username = newUsername;
                    await App.Database.UpdateAsync(user);
                    LoadUsers();
                }
            }
        }
    }
}
