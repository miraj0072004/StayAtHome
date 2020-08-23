using System;
using StayAtHome.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StayAtHome
{
    public partial class App : Application
    {
        public static string DatabaseLocation = string.Empty;

        public static MapViewModel GlobalMapViewModel = new MapViewModel();
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        public App(string databaseLocation)
        {
            InitializeComponent();

            //MainPage = new NavigationPage(new MainPage());
            MainPage = new NavigationPage(new MapPage());
            DatabaseLocation = databaseLocation;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
