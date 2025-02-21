using FotoApp.MVVM.Data;
namespace FotoApp
{
    public partial class App : Application
    {
        public static Constants Database { get; private set; }
        public App()
        {
            InitializeComponent();
            InitializeDatabase();
        }
        private void InitializeDatabase()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "FotoApp.db");
            Database = new Constants(dbPath);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}