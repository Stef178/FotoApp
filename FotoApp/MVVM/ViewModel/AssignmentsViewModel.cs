// AssignmentsViewModel.cs
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FotoApp.MVVM.Model;
using Microsoft.Maui.Controls;

namespace FotoApp.MVVM.View
{
    public class AssignmentsViewModel : INotifyPropertyChanged
    {
        private Assignment _currentAssignment;
        private string _currentPhotoPath;
        private ObservableCollection<Assignment> _upcomingAssignments;

        public AssignmentsViewModel()
        {
            _upcomingAssignments = new ObservableCollection<Assignment>();
            LoadCurrentAssignment();
            LoadUpcomingAssignments();
        }

        public Assignment CurrentAssignment
        {
            get => _currentAssignment;
            set
            {
                _currentAssignment = value;
                OnPropertyChanged(nameof(CurrentAssignment));
                OnPropertyChanged(nameof(IsAssignmentActive));
            }
        }

        public string CurrentPhotoPath
        {
            get => _currentPhotoPath;
            set
            {
                _currentPhotoPath = value;
                OnPropertyChanged(nameof(CurrentPhotoPath));
            }
        }

        public ObservableCollection<Assignment> UpcomingAssignments
        {
            get => _upcomingAssignments;
            set
            {
                _upcomingAssignments = value;
                OnPropertyChanged(nameof(UpcomingAssignments));
            }
        }

        public bool IsAssignmentActive => CurrentAssignment != null;

        private async void LoadCurrentAssignment()
        {
            var assignments = await App.Database.GetAllAsync<Assignment>();
            CurrentAssignment = assignments.FirstOrDefault(a => a.IsAvailable);
        }

        private async void LoadUpcomingAssignments()
        {
            var assignments = await App.Database.GetAllAsync<Assignment>();
            foreach (var assignment in assignments)
            {
                UpcomingAssignments.Add(assignment);
            }
        }

        public ICommand TakePhotoCommand => new Command(async () => await ExecuteTakePhoto());

        private async Task ExecuteTakePhoto()
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo == null) return;

                var stream = await photo.OpenReadAsync();
                var filePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                using (var fileStream = File.Create(filePath))
                {
                    await stream.CopyToAsync(fileStream);
                }

                // Save photo in database
                var newPhoto = new Photo { ImagePath = filePath, UserId = App.Database.GetActiveUser().Id };
                await App.Database.AddAsync(newPhoto);

                CurrentPhotoPath = filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error capturing photo: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
