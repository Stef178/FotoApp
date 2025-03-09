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
        private bool _isSaveButtonVisible;

        public AssignmentsViewModel()
        {
            _upcomingAssignments = new ObservableCollection<Assignment>();
            LoadAssignments();
        }

        public Assignment CurrentAssignment
        {
            get => _currentAssignment;
            set
            {
                _currentAssignment = value;
                OnPropertyChanged(nameof(CurrentAssignment));
                OnPropertyChanged(nameof(IsCurrentAssignmentEmpty));
                OnPropertyChanged(nameof(IsCurrentAssignmentAvailable));
            }
        }

        public string CurrentPhotoPath
        {
            get => _currentPhotoPath;
            set
            {
                _currentPhotoPath = value;
                OnPropertyChanged(nameof(CurrentPhotoPath));
                IsSaveButtonVisible = !string.IsNullOrEmpty(value);
            }
        }

        public ObservableCollection<Assignment> UpcomingAssignments
        {
            get => _upcomingAssignments;
            set
            {
                _upcomingAssignments = value;
                OnPropertyChanged(nameof(UpcomingAssignments));
                OnPropertyChanged(nameof(IsUpcomingAssignmentsEmpty));
                OnPropertyChanged(nameof(IsUpcomingAssignmentsAvailable));
            }
        }

        public bool IsSaveButtonVisible
        {
            get => _isSaveButtonVisible;
            set
            {
                _isSaveButtonVisible = value;
                OnPropertyChanged(nameof(IsSaveButtonVisible));
            }
        }

        public bool IsCurrentAssignmentEmpty => CurrentAssignment == null;
        public bool IsCurrentAssignmentAvailable => CurrentAssignment != null;
        public bool IsUpcomingAssignmentsEmpty => !UpcomingAssignments.Any();
        public bool IsUpcomingAssignmentsAvailable => UpcomingAssignments.Any();

        private async void LoadAssignments()
        {
            var assignments = await App.Database.GetAllAsync<Assignment>();
            var availableAssignments = assignments.Where(a => !a.IsCompleted).ToList();

            CurrentAssignment = availableAssignments.FirstOrDefault();
            UpcomingAssignments.Clear();
            foreach (var assignment in availableAssignments.Skip(1))
            {
                UpcomingAssignments.Add(assignment);
            }

            OnPropertyChanged(nameof(IsCurrentAssignmentEmpty));
            OnPropertyChanged(nameof(IsCurrentAssignmentAvailable));
            OnPropertyChanged(nameof(IsUpcomingAssignmentsEmpty));
            OnPropertyChanged(nameof(IsUpcomingAssignmentsAvailable));
        }

        public ICommand TakePhotoCommand => new Command(async () => await ExecuteTakePhoto());
        public ICommand SavePhotoCommand => new Command(async () => await ExecuteSavePhoto());

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

                CurrentPhotoPath = filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error capturing photo: {ex.Message}");
            }
        }

        private async Task ExecuteSavePhoto()
        {
            try
            {
                if (string.IsNullOrEmpty(CurrentPhotoPath)) return;

                var newPhoto = new Photo { ImagePath = CurrentPhotoPath, UserId = App.Database.GetActiveUser().Id };
                await App.Database.AddAsync(newPhoto);

                if (CurrentAssignment != null)
                {
                    CurrentAssignment.IsCompleted = true;
                    await App.Database.UpdateAsync(CurrentAssignment);
                }

                CurrentAssignment = null;
                IsSaveButtonVisible = false;
                CurrentPhotoPath = string.Empty;

                LoadAssignments();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving photo: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
