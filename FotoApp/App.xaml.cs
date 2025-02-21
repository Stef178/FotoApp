using FotoApp.MVVM.Data;
using FotoApp.MVVM.Model;
using FotoApp.MVVM.View;
namespace FotoApp
{
    public partial class App : Application
    {
        public static Constants Database { get; private set; }

        public static User CurrentUser { get; set; }
        public App()
        {
            InitializeComponent();
            InitializeDatabase();

            CurrentUser = Database.GetActiveUser();

            if (CurrentUser != null)
            {
                MainPage = new NavigationPage(new HomePage());
            }
            else
            {
                MainPage = new NavigationPage(new StartPage());
            }
        }
        private void InitializeDatabase()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "FotoApp.db");
            Database = new Constants(dbPath);
        }

        public static User AdminUser { get; } = new User
        {
            Id = -1,
            Username = "Admin",
            Email = "admin@admin.nl",
            Password = "admin",
            
        };
    }
}