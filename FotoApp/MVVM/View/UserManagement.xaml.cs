using System;
using FotoApp.MVVM.Model;
using Microsoft.Maui.Controls;

namespace FotoApp.MVVM.View
{
    public partial class UserManagement : ContentPage
    {
        public UserManagement()
        {
            InitializeComponent();
        }

        private async void OnChooseFrequencyClicked(object sender, EventArgs e)
        {
            var frequency = await DisplayActionSheet("Kies opdracht frequentie", "Annuleer", null, "Dagelijks", "Wekelijks");
            if (frequency != null)
            {
                var user = App.CurrentUser;
                if (user != null)
                {
                    user.Frequency = frequency == "Dagelijks" ? Frequency.Daily : Frequency.Weekly;
                    await App.Database.UpdateAsync(user);
                    await DisplayAlert("Succes", "Opdracht frequentie is bijgewerkt!", "OK");
                }
            }
        }
    }
}
