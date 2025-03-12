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
            // Haal alle foto's op
            var allPhotos = await App.Database.GetAllAsync<Photo>();
            // Filter foto's waarvan de UserId NIET gelijk is aan die van de actieve gebruiker
            var filteredPhotos = allPhotos.Where(p => p.UserId != activeUser.Id);

            Photos.Clear();

            // Haal eerst alle comments op (zodat we niet voor elke foto apart de DB moeten aanspreken)
            var allComments = await App.Database.GetAllAsync<Comment>();

            foreach (var photo in filteredPhotos)
            {
                var photoComments = allComments.Where(c => c.PhotoId == photo.Id).ToList();
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

            // Maak een nieuwe comment aan (zorg dat PhotoId niet genegeerd wordt in je model)
            var comment = new Comment
            {
                Text = NewCommentText,
                PhotoId = Photo.Id
            };

            // Sla de comment op in de database
            await App.Database.AddAsync(comment);
            Comments.Add(comment);
            NewCommentText = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
