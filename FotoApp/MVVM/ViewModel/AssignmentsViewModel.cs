using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FotoApp.MVVM.Model;
using Microsoft.Maui.Controls;
using System.Timers;
using System.ComponentModel;

namespace FotoApp.MVVM.View
{
    public class AssignmentsViewModel : INotifyPropertyChanged
    {
        private Assignment _currentAssignment;
        private string _currentPhotoPath;
        private ObservableCollection<Assignment> _upcomingAssignments;
        private bool _isSaveButtonVisible;
        private bool _isStartButtonVisible = true;
        private bool _isPhotoButtonVisible = false;
        private bool _isTimerVisible = false;
        private string _timerText = "00:00";
        private System.Timers.Timer _timer;
        private int _remainingTimeInSeconds;

        public AssignmentsViewModel()
        {
            _upcomingAssignments = new ObservableCollection<Assignment>();
            LoadAssignments();

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
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

        public bool IsStartButtonVisible
        {
            get => _isStartButtonVisible;
            set
            {
                _isStartButtonVisible = value;
                OnPropertyChanged(nameof(IsStartButtonVisible));
            }
        }

        public bool IsPhotoButtonVisible
        {
            get => _isPhotoButtonVisible;
            set
            {
                _isPhotoButtonVisible = value;
                OnPropertyChanged(nameof(IsPhotoButtonVisible));
            }
        }

        public bool IsTimerVisible
        {
            get => _isTimerVisible;
            set
            {
                _isTimerVisible = value;
                OnPropertyChanged(nameof(IsTimerVisible));
            }
        }

        public string TimerText
        {
            get => _timerText;
            set
            {
                _timerText = value;
                OnPropertyChanged(nameof(TimerText));
            }
        }

        public bool IsCurrentAssignmentEmpty => CurrentAssignment == null;
        public bool IsCurrentAssignmentAvailable => CurrentAssignment != null;
        public bool IsUpcomingAssignmentsEmpty => !UpcomingAssignments.Any();
        public bool IsUpcomingAssignmentsAvailable => UpcomingAssignments.Any();

        private async void LoadAssignments()
        {
            var activeUser = App.Database.GetActiveUser();
            var assignments = await App.Database.GetAllAsync<Assignment>();
            var completedAssignments = await App.Database.GetAllAsync<CompletedAssignment>();

            // Filter de opdrachten die de actieve gebruiker nog niet heeft voltooid.
            var availableAssignments = assignments
                .Where(a => !completedAssignments.Any(c => c.AssignmentId == a.Id && c.UserId == activeUser.Id))
                .ToList();

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
        public ICommand StartTimerCommand => new Command(async () => await StartTimer());

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

                var activeUser = App.Database.GetActiveUser();
                var newPhoto = new Photo { ImagePath = CurrentPhotoPath, UserId = activeUser.Id };
                await App.Database.AddAsync(newPhoto);

                if (CurrentAssignment != null)
                {
                    // Voeg een record toe dat de opdracht door de actieve gebruiker is voltooid.
                    var completed = new CompletedAssignment
                    {
                        AssignmentId = CurrentAssignment.Id,
                        UserId = activeUser.Id
                    };
                    await App.Database.AddAsync(completed);
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

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (CurrentAssignment != null && CurrentAssignment.IsTimerRunning)
            {
                if (_remainingTimeInSeconds > 0)
                {
                    _remainingTimeInSeconds--;
                    int minutes = _remainingTimeInSeconds / 60;
                    int seconds = _remainingTimeInSeconds % 60;
                    TimerText = $"{minutes:D2}:{seconds:D2}";
                    OnPropertyChanged(nameof(TimerText));
                }
                else
                {
                    _timer.Stop();
                    CurrentAssignment.IsTimerRunning = false;
                    OnPropertyChanged(nameof(CurrentAssignment.IsTimerRunning));
                    IsPhotoButtonVisible = false;
                }
            }
        }

        private async Task StartTimer()
        {
            if (CurrentAssignment == null) return;

            if (!CurrentAssignment.IsTimerRunning)
            {
                try
                {
                    Vibration.Vibrate(TimeSpan.FromMilliseconds(500));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Vibration failed: {ex.Message}");
                }
                CurrentAssignment.IsTimerRunning = true;
                _remainingTimeInSeconds = CurrentAssignment.DeadlineInMinutes * 60;
                _timer.Start();
                IsStartButtonVisible = false;
                IsPhotoButtonVisible = true;
                IsTimerVisible = true;
                OnPropertyChanged(nameof(CurrentAssignment.IsTimerRunning));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
