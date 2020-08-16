﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Plugin.Geolocator;
using Plugin.Toast;
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
        private Color _timeBorderColor;
        private Color _distanceBorderColor;
        private double _elapsedDistanceMeters = 0;
        private bool _journeyOngoing;


        public bool JourneyOngoing
        {
            get { return _journeyOngoing; }
            set
            {
                _journeyOngoing = value;
                OnPropertyChanged();
            }
        }


        public Color DistanceBorderColor
        {
            get { return _distanceBorderColor; }
            set
            {
                _distanceBorderColor = value;
                OnPropertyChanged();
            }
        }


        public Color TimeBorderColor
        {
            get { return _timeBorderColor; }
            set
            {
                _timeBorderColor = value;
                OnPropertyChanged();
            }
        }

        public bool TimeVibrated { get; set; } = false;
        public bool DistanceVibrated { get; set; } = false;

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



        

        public double ElapsedDistanceMeters
        {
            get { return _elapsedDistanceMeters; }
            set
            {
                _elapsedDistanceMeters = value;
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

        public MapViewModel()
        {
            StartJourneyCommand = new StartJourneyCommand(this);
            ChosenLocation = new LocalAddress();
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



            if (Settings.Address != "" && Settings.Longitude != "" && Settings.Latitude != "")
            {
                ChosenLocation.Longitude = Double.Parse(Settings.Longitude);
                ChosenLocation.Latitude = Double.Parse(Settings.Latitude);
                ChosenLocation.Address = Settings.Address;
                DisplayInMaps();
            }
            else
            {
                var locator = CrossGeolocator.Current;
                var position = await locator.GetPositionAsync();
                ChosenLocation.Longitude = position.Longitude;
                ChosenLocation.Latitude = position.Latitude;

            }

            var center = new Xamarin.Forms.Maps.Position(ChosenLocation.Latitude, ChosenLocation.Longitude);
            var span = new Xamarin.Forms.Maps.MapSpan(center, 2, 2);
            LocationMap.MoveToRegion(span);


            
        }

        public async void StopListening()
        {
            var locator = CrossGeolocator.Current;
            locator.PositionChanged -= Locator_PositionChanged;

            await locator.StopListeningAsync();
        }

        public async void StartListening()
        {
            if (!JourneyOngoing)
            {
                if (Settings.Address != "" && Settings.Longitude != "" && Settings.Latitude != "")
                {
                    var locator = CrossGeolocator.Current;
                    locator.PositionChanged += Locator_PositionChanged;

                    if (!CrossGeolocator.Current.IsListening)
                    {
                        await locator.StartListeningAsync(new TimeSpan(1), .1);
                    }

                    Device.StartTimer(TimeSpan.FromSeconds(1), HandleTime );
                    JourneyOngoing = true;
                }
                else
                {
                    CrossToastPopUp.Current.ShowToastMessage("Set the location address to continue");
                    await Application.Current.MainPage.Navigation.PushAsync(new SearchPage());
                }
            }
            else
            {
                JourneyOngoing = false;
                StopListening();
            }
        }

        private bool HandleTime()
        {
            if (ElapsedSeconds < 59)
            {
                ElapsedSeconds++;

                if (TimeBorderColor != Color.OrangeRed)
                {
                    if (ElapsedSeconds<40)
                    {
                        if (TimeBorderColor != Color.YellowGreen)
                        {
                            TimeBorderColor = Color.YellowGreen;
                        }
                    }
                    else
                    {
                        if (TimeBorderColor != Color.Yellow)
                        {
                            TimeBorderColor = Color.Yellow;
                        }
                    }
                }
            }
            else
            {
                ElapsedSeconds = 0;

                if (ElapsedMinutes<59)
                {
                    ElapsedMinutes++;

                    if (ElapsedMinutes==1)
                    {
                        if (!TimeVibrated)
                        {
                            try
                            {
                                // Use default vibration length
                                Vibration.Vibrate();
                                Application.Current.MainPage.DisplayAlert("Stage 4 Violation", "You have traveled for more than one hour", "Ok");

                                // Or use specified time
                                var duration = TimeSpan.FromSeconds(1);
                                Vibration.Vibrate(duration);
                                TimeVibrated = true;
                                TimeBorderColor = Color.OrangeRed;
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

            //ElapsedDistanceMeters = "Distance moved " + elapsedDistanceNumber * 1000 + " meters";

            //var elapsedDistanceMetersDouble = elapsedDistanceNumber * 1000;

            ElapsedDistanceMeters = elapsedDistanceNumber * 1000;
            


            if (ElapsedDistanceMeters > 100)
            {
                //var reply = DisplayAlert("Moved", "Moved more than 25 meters", "Ok");

                //if (reply)
                //{

                //}

                if (!DistanceVibrated)
                {
                    try
                    {
                        // Use default vibration length
                        Vibration.Vibrate();
                        Application.Current.MainPage.DisplayAlert("Stage 4 Violation", "You have traveled more than 5 kilometers", "Ok");

                        // Or use specified time
                        var duration = TimeSpan.FromSeconds(1);
                        Vibration.Vibrate(duration);
                        DistanceVibrated = true;

                        if (DistanceBorderColor != Color.OrangeRed)
                        {
                            DistanceBorderColor = Color.OrangeRed;
                        }
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
            else
            {
                if (DistanceVibrated)
                {
                    DistanceVibrated = false;
                }

                if (ElapsedDistanceMeters < 80)
                {
                    if (DistanceBorderColor != Color.YellowGreen)
                    {
                        DistanceBorderColor = Color.YellowGreen;
                    }
                }
                else
                {
                    if (DistanceBorderColor != Color.Yellow)
                    {
                        DistanceBorderColor = Color.Yellow;
                    }
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
