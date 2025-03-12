using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FotoApp.MVVM.Model;
using FotoApp.MVVM.Data;
using Microsoft.Maui.Controls;

namespace FotoApp.MVVM.ViewModel
{
    public class UserPhotosViewModel : BaseViewModel
    {
        private ObservableCollection<Photo> _userPhotos;
        public ObservableCollection<Photo> UserPhotos
        {
            get => _userPhotos;
            set => SetProperty(ref _userPhotos, value);
        }

        public ICommand DeletePhotoCommand { get; }

        public UserPhotosViewModel()
        {
            LoadUserPhotos();
            DeletePhotoCommand = new Command<string>(async (photoPath) => await DeletePhoto(photoPath));
        }

        private async void LoadUserPhotos()
        {
            var activeUser = App.Database.GetActiveUser();
            var userId = activeUser.Id; // Huidige gebruiker ophalen
            var photos = await App.Database.GetAllAsync<Photo>();
            var userPhotos = photos.Where(p => p.UserId == userId).ToList();

            // Haal alle reacties en gebruikers op
            var allComments = await App.Database.GetAllAsync<Comment>();
            var allUsers = await App.Database.GetAllAsync<User>();

            // Voor iedere foto de reacties (van anderen) laden en de User-property instellen
            foreach (var photo in userPhotos)
            {
                var photoComments = allComments
                                    .Where(c => c.PhotoId == photo.Id && c.UserId != photo.UserId)
                                    .ToList();
                foreach (var comment in photoComments)
                {
                    comment.User = allUsers.FirstOrDefault(u => u.Id == comment.UserId);
                }
                photo.Comments = photoComments;
            }

            UserPhotos = new ObservableCollection<Photo>(userPhotos);
        }

        private async Task DeletePhoto(string photoPath)
        {
            var photos = await App.Database.GetAllAsync<Photo>();
            var photo = photos.FirstOrDefault(p => p.ImagePath == photoPath);

            if (photo != null)
            {
                await App.Database.DeleteAsync(photo);
                LoadUserPhotos(); // Lijst opnieuw laden
            }
        }
    }
}
