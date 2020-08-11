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

        public MapPage(MapViewModel mapViewModel)
        {
            _mapViewModel = mapViewModel;
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            

            base.OnAppearing();

            var locationPermissionStatus = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.LocationWhenInUse());
            var vibratePermissionStatus = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.Vibrate());
            if (locationPermissionStatus != PermissionStatus.Granted || vibratePermissionStatus != PermissionStatus.Granted)
            {
                // Notify user permission was denied
                await DisplayAlert("Permission Denied","The app cannot continue without the location and vibration permissions","Ok");
                return;
            }

            var locator = CrossGeolocator.Current;
            locator.PositionChanged += Locator_PositionChanged;

            if (!CrossGeolocator.Current.IsListening)
            {
                await locator.StartListeningAsync(new TimeSpan(1), .1);
            }

            //var position = await locator.GetPositionAsync();

            var center = new Xamarin.Forms.Maps.Position(_mapViewModel.ChosenLocation.Latitude, _mapViewModel.ChosenLocation.Longitude);
            var span = new Xamarin.Forms.Maps.MapSpan(center, 2, 2);
            locationMap.MoveToRegion(span);



            DisplayInMaps();
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
            var locator = CrossGeolocator.Current;
            locator.PositionChanged -= Locator_PositionChanged;

            await locator.StopListeningAsync();
        }

        private void DisplayInMaps()
        {
            
                try
                {
                    var position = new Xamarin.Forms.Maps.Position(_mapViewModel.ChosenLocation.Latitude, _mapViewModel.ChosenLocation.Longitude);

                    var pin = new Xamarin.Forms.Maps.Pin
                    {
                        Type = Xamarin.Forms.Maps.PinType.SavedPin,
                        Position = position,
                        Label = _mapViewModel.ChosenLocation.Name,
                        Address = _mapViewModel.ChosenLocation.Address
                    };

                    locationMap.Pins.Add(pin);
                }
                catch (NullReferenceException nre)
                {

                }
                catch (Exception ex)
                {

                }
            
        }

        private void Locator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            var center = new Xamarin.Forms.Maps.Position(e.Position.Latitude, e.Position.Longitude);
            //var span = new Xamarin.Forms.Maps.MapSpan(center, 1, 1);
            //locationMap.MoveToRegion(span);

            //zoom differently
            double zoomLevel = 16;
            double latlongDegrees = 360 / (Math.Pow(2, zoomLevel));
            var span = new Xamarin.Forms.Maps.MapSpan(center, latlongDegrees, latlongDegrees);
            locationMap.MoveToRegion(span);

            var elapsedDistance = Location.CalculateDistance(
                _mapViewModel.ChosenLocation.Latitude,
                _mapViewModel.ChosenLocation.Longitude,
                e.Position.Latitude, e.Position.Longitude,
                DistanceUnits.Kilometers);

            this.elapsedDistance.Text = "Distance moved " + elapsedDistance*1000 + " meters";

            if (elapsedDistance*1000 > 30)
            {
                //var reply = DisplayAlert("Moved", "Moved more than 25 meters", "Ok");

                //if (reply)
                //{

                //}

                try
                {
                    // Use default vibration length
                    Vibration.Vibrate();
                    DisplayAlert("Stage 4 Violation", "You have traveled more than 5 kilometers", "Ok");

                    // Or use specified time
                    var duration = TimeSpan.FromSeconds(1);
                    Vibration.Vibrate(duration);
                }
                catch (FeatureNotSupportedException ex)
                {
                    // Feature not supported on device
                }
                catch (Exception ex)
                {
                    // Other error has occurred.
                }
            }
        }
    }
}