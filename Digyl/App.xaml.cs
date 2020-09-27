using System;
using System.IO;
using Xamarin.Forms;

namespace Digyl
{
    public partial class App : Application
    {
        static Settings settings;
        static Database database;
        static LocationUpdateHandler locationHandler;

        public static Settings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = new Settings();
                }
                return settings;
            }
        }

        public static Database Database
        {
            get
            {
                if (database == null)
                {
                    database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "database_1.db3"));
                }
                return database;
            }
        }

        public static LocationUpdateHandler LocationUpdateHandler
        {
            get
            {
                if (locationHandler == null)
                {
                    locationHandler = new LocationUpdateHandler();
                    //locationHandler.StartLocationHandler();
                }
                return locationHandler;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            Application.Current.Resources["historyButtonVisible"] = Settings.ManualHistoryEnabled;
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
