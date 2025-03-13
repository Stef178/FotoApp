using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using FotoApp.MVVM.Model;
using Microsoft.Maui.Controls;

namespace FotoApp.MVVM.ViewModel
{
    public class UserManagementViewModel : INotifyPropertyChanged
    {
        private User _currentUser;
        private ObservableCollection<Frequency> _frequencies;

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PointsBalance));
                OnPropertyChanged(nameof(IsSuperMember));
                OnPropertyChanged(nameof(SuperMembershipStatus)); 
            }
        }

        public int PointsBalance => CurrentUser?.PointsBalance ?? 0;

        public bool IsSuperMember => CurrentUser?.IsSuperMember ?? false;

        public string SuperMembershipStatus => IsSuperMember ? "Je hebt een superlidmaatschap." : "Je hebt geen superlidmaatschap.";

        public ObservableCollection<Frequency> Frequencies
        {
            get => _frequencies;
            set
            {
                _frequencies = value;
                OnPropertyChanged();
            }
        }

        public ICommand BuyPointsCommand => new Command(async () => await ConfirmBuyPointsAsync());
        public ICommand BuySuperMembershipCommand => new Command(async () => await ConfirmBuySuperMembershipAsync());
        public ICommand CancelSuperMembershipCommand => new Command(async () => await CancelSuperMembershipAsync());

        public UserManagementViewModel()
        {
            LoadUserData();
            LoadFrequencies();
        }

        private void LoadUserData()
        {
            CurrentUser = App.CurrentUser ?? new User { PointsBalance = 0, IsSuperMember = false };
        }

        private void LoadFrequencies()
        {
            Frequencies = new ObservableCollection<Frequency>
            {
                Frequency.Dagelijks,
                Frequency.Wekelijks,
            };
        }

        private async Task ConfirmBuyPointsAsync()
        {
            bool confirmed = await Application.Current.MainPage.DisplayAlert("Bevestiging", "Weet je het zeker dat je punten wilt kopen?", "Ja", "Nee");
            if (confirmed)
            {
                await BuyPointsAsync();
            }
        }

        private async Task ConfirmBuySuperMembershipAsync()
        {
            bool confirmed = await Application.Current.MainPage.DisplayAlert("Bevestiging", "Weet je het zeker dat je een superlidmaatschap wilt kopen?", "Ja", "Nee");
            if (confirmed)
            {
                await BuySuperMembershipAsync();
            }
        }

        public async Task BuyPointsAsync()
        {
            if (CurrentUser == null) return;

            CurrentUser.PointsBalance += 1;
            await App.Database.UpdateAsync(CurrentUser);
            OnPropertyChanged(nameof(PointsBalance));
        }

        public async Task BuySuperMembershipAsync()
        {
            if (CurrentUser == null || CurrentUser.IsSuperMember) return;

            CurrentUser.IsSuperMember = true;
            CurrentUser.PointsBalance += 5;

            var transaction = new Transaction
            {
                UserId = CurrentUser.Id,
                Points = 5,
                AmountPaid = 5m,
                Date = DateTime.Now
            };

            await App.Database.AddAsync(transaction);
            await App.Database.UpdateAsync(CurrentUser);
            OnPropertyChanged(nameof(IsSuperMember));
            OnPropertyChanged(nameof(PointsBalance));
            OnPropertyChanged(nameof(SuperMembershipStatus)); 
        }

        public async Task CancelSuperMembershipAsync()
        {
            if (CurrentUser == null || !CurrentUser.IsSuperMember) return;

            CurrentUser.IsSuperMember = false;
            await App.Database.UpdateAsync(CurrentUser);
            OnPropertyChanged(nameof(IsSuperMember));
            OnPropertyChanged(nameof(SuperMembershipStatus)); 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
