using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Plugin.Geolocator;
using Plugin.LocalNotification;
using Plugin.Toast;
using StayAtHome.Annotations;
using StayAtHome.Commands;
using StayAtHome.Helpers;
using StayAtHome.Messages;
using StayAtHome.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Map = Xamarin.Forms.Maps.Map;


namespace StayAtHome.ViewModels
{
    enum ViolationType{
    Distance,
    Time
    }
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
        INotificationManager notificationManager;


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
            DistanceBorderColor = TimeBorderColor = Color.Black;

            HandleReceivedMessages(); //Handle service messages

            notificationManager = DependencyService.Get<INotificationManager>();

            //notificationManager.NotificationReceived += (sender, eventArgs) =>
            //{
            //    var evtData = (NotificationEventArgs)eventArgs;
            //    ShowNotification(evtData.Title, evtData.Message);
            //};
            
        }

        public void SendNotification(string title, string message)
        {
            notificationManager.ScheduleNotification(title, message);
        }

        public void ShowNotification(string title, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var msg = new Label()
                {
                    Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
                };
                //stackLayout.Children.Add(msg);
            });
        }

        private void HandleReceivedMessages()
        {
            MessagingCenter.Subscribe<TickedMessage>(this, "StartJourneyMessage", message => {
                Device.BeginInvokeOnMainThread(StartListening);
            });

            MessagingCenter.Subscribe<CancelledMessage>(this, "StopJourneyMessage", message => {
                Device.BeginInvokeOnMainThread(StopListening);
            });
        }


        public async void InitializeMap()
        {
            
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

            SetMapStartPoint();
        }

        private void SetMapStartPoint()
        {
            var center = new Xamarin.Forms.Maps.Position(ChosenLocation.Latitude, ChosenLocation.Longitude);
            var span = new Xamarin.Forms.Maps.MapSpan(center, 2, 2);
            LocationMap.MoveToRegion(span);
        }

        public async void StopListening()
        {
            var locator = CrossGeolocator.Current;
            locator.PositionChanged -= Locator_PositionChanged;
            ResetTimerAndDistance();
            SetMapStartPoint();

            await locator.StopListeningAsync();
        }

        private void ResetTimerAndDistance()
        {
            ElapsedHours = ElapsedMinutes = ElapsedSeconds = 0;
            ElapsedDistanceMeters = 0;
            DistanceBorderColor = TimeBorderColor = Color.Black;
        }

        public async void StartListening()
        {
            //if (!JourneyOngoing)
            //{
            //    if (Settings.Address != "" && Settings.Longitude != "" && Settings.Latitude != "")
            //    {
            //        var locator = CrossGeolocator.Current;
            //        locator.PositionChanged += Locator_PositionChanged;

            //        if (!CrossGeolocator.Current.IsListening)
            //        {
            //            await locator.StartListeningAsync(new TimeSpan(1), .1);
            //        }
            //        JourneyOngoing = true;
            //        Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            //        {
            //            if (JourneyOngoing)
            //            {
            //                HandleTime();
            //                return true;
            //            }

            //            return false;
            //        });

            //    }
            //    else
            //    {
            //        CrossToastPopUp.Current.ShowToastMessage("Set the location address to continue");
            //        await Application.Current.MainPage.Navigation.PushAsync(new SearchPage());
            //    }
            //}
            //else
            //{
            //    JourneyOngoing = false;
            //    StopListening();
            //}

            //var message = new StartLongRunningTaskMessage();
            //MessagingCenter.Send(message, "StartLongRunningTaskMessage");

            if (Settings.Address != "" && Settings.Longitude != "" && Settings.Latitude != "")
            {
                var locator = CrossGeolocator.Current;
                locator.PositionChanged += Locator_PositionChanged;

                if (!CrossGeolocator.Current.IsListening)
                {
                    await locator.StartListeningAsync(new TimeSpan(1), .1);
                }
                JourneyOngoing = true;
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    if (JourneyOngoing)
                    {
                        HandleTime();
                        return true;
                    }

                    return false;
                });

            }
            else
            {
                CrossToastPopUp.Current.ShowToastMessage("Set the location address to continue");
                await Application.Current.MainPage.Navigation.PushAsync(new SearchPage());
            }
        }

        public void StartJourneyCommandExecute()
        {
            if (!JourneyOngoing)
            {
                var message = new StartLongRunningTaskMessage();
                MessagingCenter.Send(message, "StartLongRunningTaskMessage");
                JourneyOngoing = true;
            }
            else
            {
                var message = new StopLongRunningTaskMessage();
                MessagingCenter.Send(message, "StopLongRunningTaskMessage");
                JourneyOngoing = false;
            }
        }

        private void HandleTime()
        {
            if (ElapsedSeconds < 59)
            {
                ElapsedSeconds++;
                if (TimeBorderColor != Color.OrangeRed)
                {
                    if (ElapsedSeconds < 6)
                    {
                        if (TimeBorderColor != Color.YellowGreen)
                        {
                            TimeBorderColor = Color.YellowGreen;
                        }
                    }
                    else
                    {
                        if (TimeBorderColor != Color.Yellow || TimeBorderColor != Color.OrangeRed)
                        {
                            TimeBorderColor = Color.Yellow;
                        }
                    }
                }

                if (ElapsedSeconds == 10)
                {
                    if (!TimeVibrated)
                    {
                        try
                        {
                            // Use default vibration length
                            //Vibration.Vibrate();
                            NotifyLockdownViolation(ViolationType.Time);
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
                ElapsedSeconds = 0;

                if (ElapsedMinutes<59)
                {
                    ElapsedMinutes++;

                    //if (ElapsedMinutes==1)
                    //{
                    //    if (!TimeVibrated)
                    //    {
                    //        try
                    //        {
                    //            // Use default vibration length
                    //            //Vibration.Vibrate();
                    //            NotifyLockdownViolation(ViolationType.Time);
                    //            TimeBorderColor = Color.OrangeRed;
                    //        }
                    //        catch (FeatureNotSupportedException ex)
                    //        {
                    //            // Feature not supported on device
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            // Other error has occurred.
                    //        }
                    //    }
                    //}
                }
                else
                {
                    ElapsedMinutes = 0;
                    ElapsedHours++;
                }
            }

            
        }

        private void NotifyLockdownViolation(ViolationType violationType)
        {
            var description = "";
            if (violationType == ViolationType.Distance)
            {
                description = "You have traveled for more than 5 km's";
                Application.Current.MainPage.DisplayAlert("Stage 4 Violation",description , "Ok");
                TimeVibrated = true;
            }
            else
            {
                description = "You have traveled for more than one hour";
                Application.Current.MainPage.DisplayAlert("Stage 4 Violation",description , "Ok");
                DistanceVibrated = true;
            }
            
            // Or use specified time
            var duration = TimeSpan.FromSeconds(2);
            Vibration.Vibrate(duration);

            //var notification = new NotificationRequest
            //{
            //    NotificationId = 100,
            //    Title = "Stage 4 violation",
            //    Description = description,
            //    ReturningData = "Dummy data", // Returning data when tapped on notification.
            //    NotifyTime = DateTime.Now.AddSeconds(30) // Used for Scheduling local notification, if not specified notification will show immediately.
            //};
            //NotificationCenter.Current.Show(notification);

            SendNotification("Stage 4 Violation",description);
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
                        NotifyLockdownViolation(ViolationType.Distance);

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
