namespace FotoApp.MVVM.View
{
    public partial class CommentsPage : ContentPage
    {
        public CommentsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is ViewModel.CommentsViewModel viewModel)
            {
                await viewModel.LoadPhotos();
            }
        }
    }
}
