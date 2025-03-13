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
        
        public ObservableCollection<PhotoItemViewModel> Photos { get; set; } = new ObservableCollection<PhotoItemViewModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        
        public async Task LoadPhotos()
        {
            
            var activeUser = App.Database.GetActiveUser();

           
            var allPhotos = await App.Database.GetAllAsync<Photo>();
            var allUsers = await App.Database.GetAllAsync<User>();

            
            var filteredPhotos = allPhotos.Where(p => p.UserId != activeUser.Id).ToList();

            Photos.Clear();

            
            var allComments = await App.Database.GetAllAsync<Comment>();

            foreach (var photo in filteredPhotos)
            {
                
                photo.User = allUsers.FirstOrDefault(u => u.Id == photo.UserId);

                
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

       
        public ObservableCollection<Comment> Comments { get; set; } = new ObservableCollection<Comment>();

        
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

            
            var activeUser = App.CurrentUser;
            if (activeUser == null)
                return; 

            
            var comment = new Comment
            {
                Text = NewCommentText,
                PhotoId = Photo.Id,
                UserId = activeUser.Id
            };

            
            await App.Database.AddAsync(comment);

            
            comment.User = activeUser;

            Comments.Add(comment);
            NewCommentText = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
