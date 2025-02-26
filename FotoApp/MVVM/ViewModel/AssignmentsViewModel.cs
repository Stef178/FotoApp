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
            }
        }

        public bool IsAssignmentActive => CurrentAssignment != null;

        public bool IsSaveButtonVisible
        {
            get => _isSaveButtonVisible;
            set
            {
                _isSaveButtonVisible = value;
                OnPropertyChanged(nameof(IsSaveButtonVisible));
            }
        }

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

                // Markeer de opdracht als voltooid en update de database
                if (CurrentAssignment != null)
                {
                    CurrentAssignment.IsCompleted = true;
                    await App.Database.UpdateAsync(CurrentAssignment);
                }

                // Opdracht verwijderen uit de UI
                CurrentAssignment = null;
                IsSaveButtonVisible = false;
                CurrentPhotoPath = string.Empty;

                // Laad de lijst opnieuw om de volgende opdracht te tonen
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
