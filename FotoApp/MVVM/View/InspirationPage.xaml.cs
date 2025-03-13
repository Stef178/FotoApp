using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace FotoApp.MVVM.View
{
    public partial class InspirationPage : ContentPage
    {
        public ObservableCollection<PexelsPhoto> Photos { get; set; } = new ObservableCollection<PexelsPhoto>();

        public InspirationPage()
        {
            InitializeComponent();
            PhotosCollectionView.ItemsSource = Photos;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadInspirationPhotosAsync();
        }

        private async Task LoadInspirationPhotosAsync()
        {
            try
            {
                
                string apiKey = "0B2Ft17LjCWlp0nR6PuuabmKge3ttjJ7a1B1UqDAQRPEgLUQ0xqQvwzt";
                string url = "https://api.pexels.com/v1/curated?per_page=10";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", apiKey);
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        using (JsonDocument doc = JsonDocument.Parse(json))
                        {
                            var root = doc.RootElement;
                            if (root.TryGetProperty("photos", out JsonElement photosArray))
                            {
                                Photos.Clear();
                                foreach (var photoElement in photosArray.EnumerateArray())
                                {
                                    string photoUrl = "";
                                    string photographer = "";
                                    if (photoElement.TryGetProperty("src", out JsonElement srcElement))
                                    {
                                        photoUrl = srcElement.GetProperty("medium").GetString();
                                    }
                                    if (photoElement.TryGetProperty("photographer", out JsonElement photographerElement))
                                    {
                                        photographer = photographerElement.GetString();
                                    }
                                    Photos.Add(new PexelsPhoto
                                    {
                                        PhotoUrl = photoUrl,
                                        Photographer = photographer
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", $"Fout bij laden inspiratie foto's ({response.StatusCode})", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    public class PexelsPhoto
    {
        public string PhotoUrl { get; set; }
        public string Photographer { get; set; }
    }
}
