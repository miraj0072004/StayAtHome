using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Geolocator;
using StayAtHome.ViewModels;
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

            var locator = CrossGeolocator.Current;
            locator.PositionChanged += Locator_PositionChanged;

            await locator.StartListeningAsync(new TimeSpan(0), 1);

            var position = await locator.GetPositionAsync();

            var center = new Xamarin.Forms.Maps.Position(_mapViewModel.ChosenLocation.Latitude, _mapViewModel.ChosenLocation.Longitude);
            var span = new Xamarin.Forms.Maps.MapSpan(center, 2, 2);
            locationMap.MoveToRegion(span);



            DisplayInMaps();
        }
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
            var span = new Xamarin.Forms.Maps.MapSpan(center, 2, 2);
            locationMap.MoveToRegion(span);
        }
    }
}