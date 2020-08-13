using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Plugin.Geolocator;
using StayAtHome.Annotations;
using StayAtHome.Commands;
using StayAtHome.Helpers;
using StayAtHome.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Map = Xamarin.Forms.Maps.Map;


namespace StayAtHome.ViewModels
{
    public class MapViewModel : INotifyPropertyChanged
    {

        
        //private double _longitude;
        //private double _latitude;

        //public double Latitude
        //{
        //    get { return _latitude; }
        //    set { _latitude = value; }
        //}


        //public double Longitude
        //{
        //    get { return _longitude; }
        //    set { _longitude = value; }
        //}

        private LocalAddress _chosenLocation;
        private Map _locationMap;
        private int _elapsedSeconds=0;
        private int _elapsedMinutes=0;
        private int _elapsedHours=0;

        public int ElapsedHours
        {
            get { return _elapsedHours; }
            set
            {
                _elapsedHours = value;
                OnPropertyChanged();
            }
        }


        public int ElapsedMinutes
        {
            get { return _elapsedMinutes; }
            set
            {
                _elapsedMinutes = value;
                OnPropertyChanged();
            }
        }


        public int ElapsedSeconds
        {
            get { return _elapsedSeconds; }
            set
            {
                _elapsedSeconds = value;
                OnPropertyChanged();
            }
        }



        private string _elapsedDistance;

        public string ElapsedDistance
        {
            get { return _elapsedDistance; }
            set
            {
                _elapsedDistance = value;
                OnPropertyChanged();
            }
        }


        public Map LocationMap
        {
            get { return _locationMap; }
            set { _locationMap = value; }
        }


        public LocalAddress ChosenLocation
        {
            get { return _chosenLocation; }
            set { _chosenLocation = value; }
        }

        public StartJourneyCommand StartJourneyCommand { get; set; }


        public MapViewModel(LocalAddress chosenLocation)
        {
            ChosenLocation = chosenLocation;
            StartJourneyCommand = new StartJourneyCommand(this);
        }

        public async void InitializeMap(Map locationMap)
        {
            LocationMap = locationMap;
            var locationPermissionStatus = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.LocationWhenInUse());
            var vibratePermissionStatus = await PermissionHelper.CheckAndRequestPermissionAsync(new Permissions.Vibrate());
            if (locationPermissionStatus != PermissionStatus.Granted || vibratePermissionStatus != PermissionStatus.Granted)
            {
                // Notify user permission was denied
                await Application.Current.MainPage.DisplayAlert("Permission Denied", "The app cannot continue without the location and vibration permissions", "Ok");
                return;
            }

           

            //var position = await locator.GetPositionAsync();

            var center = new Xamarin.Forms.Maps.Position(ChosenLocation.Latitude, ChosenLocation.Longitude);
            var span = new Xamarin.Forms.Maps.MapSpan(center, 2, 2);
            LocationMap.MoveToRegion(span);


            DisplayInMaps();
        }

        public async void StopListening()
        {
            var locator = CrossGeolocator.Current;
            locator.PositionChanged -= Locator_PositionChanged;

            await locator.StopListeningAsync();
        }

        public async void StartListening()
        {
            var locator = CrossGeolocator.Current;
            locator.PositionChanged += Locator_PositionChanged;

            if (!CrossGeolocator.Current.IsListening)
            {
                await locator.StartListeningAsync(new TimeSpan(1), .1);
            }

            Device.StartTimer(TimeSpan.FromSeconds(1), HandleTime );
        }

        private bool HandleTime()
        {
            if (ElapsedSeconds < 60)
            {
                ElapsedSeconds++;
            }
            else
            {
                ElapsedSeconds = 0;

                if (ElapsedMinutes<60)
                {
                    ElapsedMinutes++;
                }
                else
                {
                    ElapsedMinutes = 0;
                    ElapsedHours++;
                }
            }

            return true;
        }

        private void DisplayInMaps()
        {

            try
            {
                var position = new Xamarin.Forms.Maps.Position(ChosenLocation.Latitude, ChosenLocation.Longitude);

                var pin = new Xamarin.Forms.Maps.Pin
                {
                    Type = Xamarin.Forms.Maps.PinType.SavedPin,
                    Position = position,
                    Label = ChosenLocation.Name,
                    Address = ChosenLocation.Address
                };

                LocationMap.Pins.Add(pin);
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
            LocationMap.MoveToRegion(span);

            var elapsedDistanceNumber = Location.CalculateDistance(
                ChosenLocation.Latitude,
                ChosenLocation.Longitude,
                e.Position.Latitude, e.Position.Longitude,
                DistanceUnits.Kilometers);

            //ElapsedDistance = "Distance moved " + elapsedDistanceNumber * 1000 + " meters";
            ElapsedDistance = (elapsedDistanceNumber * 1000).ToString(CultureInfo.InvariantCulture) ;

            if (elapsedDistanceNumber * 1000 > 1000)
            {
                //var reply = DisplayAlert("Moved", "Moved more than 25 meters", "Ok");

                //if (reply)
                //{

                //}

                try
                {
                    // Use default vibration length
                    Vibration.Vibrate();
                    Application.Current.MainPage.DisplayAlert("Stage 4 Violation", "You have traveled more than 5 kilometers", "Ok");

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
