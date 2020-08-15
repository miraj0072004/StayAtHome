using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Geolocator;
using StayAtHome.Helpers;
using StayAtHome.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StayAtHome
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        private readonly MapViewModel _mapViewModel;

        //uncomment if multiple address saving is needed
        //public MapPage(MapViewModel mapViewModel)
        //{
        //    _mapViewModel = mapViewModel;
        //    BindingContext = _mapViewModel;
        //    InitializeComponent();
        //}

        public MapPage()
        {
            _mapViewModel = new MapViewModel();
            BindingContext = _mapViewModel;
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            

            base.OnAppearing();

            _mapViewModel.InitializeMap(locationMap);
        }

        //public async Task<PermissionStatus> CheckAndRequestLocationPermission()
        //{
        //    var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
        //    if (status != PermissionStatus.Granted)
        //    {
        //        status = await Permissions.RequestAsync<Permissions.LocationAlways>();
        //    }

        //    // Additionally could prompt the user to turn on in settings

        //    return status;
        //}
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            _mapViewModel.StopListening();
        }


        private void AddressButton_OnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SearchPage());
        }
    }
}