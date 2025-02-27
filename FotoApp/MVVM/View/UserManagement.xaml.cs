using Microsoft.Maui.Controls;
using FotoApp.MVVM.ViewModel;

namespace FotoApp.MVVM.View
{
    public partial class UserManagement : ContentPage
    {
        private readonly UserManagementViewModel _viewModel;

        public UserManagement()
        {
            InitializeComponent();
            _viewModel = new UserManagementViewModel();
            BindingContext = _viewModel;
        }
    }
}
