using Microsoft.Maui.Controls;

namespace FotoApp.MVVM.View
{
    public partial class Assignments : ContentPage
    {
        public AssignmentsViewModel ViewModel { get; set; }

        public Assignments()
        {
            InitializeComponent();
            ViewModel = new AssignmentsViewModel();
            BindingContext = ViewModel;
        }
    }
}
