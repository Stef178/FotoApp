using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace FotoApp.MVVM.View
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadWeatherAsync();
        }

        private async Task LoadWeatherAsync()
        {
            try
            {
               
                string apiKey = "f10226c44c4855a55cac1d73b84c0cbc";
                string city = "Heerlen,NL";
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        using (JsonDocument doc = JsonDocument.Parse(json))
                        {
                            var root = doc.RootElement;
                            double temp = root.GetProperty("main").GetProperty("temp").GetDouble();
                            string description = "";
                            if (root.TryGetProperty("weather", out JsonElement weatherArray) &&
                                weatherArray.GetArrayLength() > 0)
                            {
                                description = weatherArray[0].GetProperty("description").GetString();
                            }
                            WeatherLabel.Text = $"{city}: {temp:F1}°C, {description}";
                        }
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        WeatherLabel.Text = $"Weer info niet beschikbaar ({response.StatusCode}): {errorContent}";
                    }
                }
            }
            catch (Exception ex)
            {
                WeatherLabel.Text = $"Error: {ex.Message}";
            }
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

        
        private async void OnInspirationClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InspirationPage());
        }
    }
}
