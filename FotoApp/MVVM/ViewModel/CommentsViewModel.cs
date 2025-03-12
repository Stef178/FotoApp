using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FotoApp.MVVM.Model;
using Microsoft.Maui.Controls;

namespace FotoApp.MVVM.ViewModel
{
    public class CommentsViewModel : INotifyPropertyChanged
    {
        // Observable collectie met de foto-items
        public ObservableCollection<PhotoItemViewModel> Photos { get; set; } = new ObservableCollection<PhotoItemViewModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        // Deze methode wordt aangeroepen wanneer de pagina verschijnt
        public async Task LoadPhotos()
        {
            // Haal de actieve gebruiker op
            var activeUser = App.Database.GetActiveUser();

            // Haal alle foto's en gebruikers op
            var allPhotos = await App.Database.GetAllAsync<Photo>();
            var allUsers = await App.Database.GetAllAsync<User>();

            // Filter foto's waarvan de UserId NIET gelijk is aan die van de actieve gebruiker
            var filteredPhotos = allPhotos.Where(p => p.UserId != activeUser.Id).ToList();

            Photos.Clear();

            // Haal eerst alle comments op
            var allComments = await App.Database.GetAllAsync<Comment>();

            foreach (var photo in filteredPhotos)
            {
                // Vul de user voor de foto
                photo.User = allUsers.FirstOrDefault(u => u.Id == photo.UserId);

                // Filter de comments voor deze foto en vul de user-informatie
                var photoComments = allComments.Where(c => c.PhotoId == photo.Id).ToList();
                foreach (var comment in photoComments)
                {
                    comment.User = allUsers.FirstOrDefault(u => u.Id == comment.UserId);
                }
                Photos.Add(new PhotoItemViewModel(photo, photoComments));
            }
        }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class PhotoItemViewModel : INotifyPropertyChanged
    {
        public Photo Photo { get; set; }

        // Lijst met comments voor deze foto
        public ObservableCollection<Comment> Comments { get; set; } = new ObservableCollection<Comment>();

        // Nieuwe comment die de gebruiker intypt
        private string _newCommentText;
        public string NewCommentText
        {
            get => _newCommentText;
            set
            {
                _newCommentText = value;
                OnPropertyChanged(nameof(NewCommentText));
            }
        }

        // Command dat wordt uitgevoerd als op "Reageer" wordt gedrukt
        public ICommand AddCommentCommand { get; }

        public PhotoItemViewModel(Photo photo, System.Collections.Generic.List<Comment> comments)
        {
            Photo = photo;
            foreach (var comment in comments)
            {
                Comments.Add(comment);
            }
            AddCommentCommand = new Command(async () => await AddComment());
        }

        private async Task AddComment()
        {
            if (string.IsNullOrWhiteSpace(NewCommentText))
                return;

            // Zorg dat de actieve gebruiker beschikbaar is (bijv. via App.CurrentUser)
            var activeUser = App.CurrentUser;
            if (activeUser == null)
                return; // of geef een foutmelding

            // Maak een nieuwe comment aan en vul de UserId in
            var comment = new Comment
            {
                Text = NewCommentText,
                PhotoId = Photo.Id,
                UserId = activeUser.Id
            };

            // Sla de comment op in de database
            await App.Database.AddAsync(comment);

            // Stel de User property in zodat de naam direct beschikbaar is
            comment.User = activeUser;

            Comments.Add(comment);
            NewCommentText = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
