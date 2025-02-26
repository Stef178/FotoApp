using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FotoApp.MVVM.Model;
using FotoApp.MVVM.View;
using FotoApp.MVVM.Data;

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
            var userId = App.Database.GetActiveUser().Id; // Huidige gebruiker ophalen
            var photos = await App.Database.GetAllAsync<Photo>();
            var userPhotos = photos.Where(p => p.UserId == userId).ToList();

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
