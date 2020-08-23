using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Toast;
using StayAtHome.Messages;
using StayAtHome.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace StayAtHome.Helpers
{
    public class JourneyHelper
    {
        private LocalAddress _chosenLocation;

        public bool JourneyOngoing { get; set; } = false;
        public LocalAddress ChosenLocation
        {
            get { return _chosenLocation; }
            set { _chosenLocation = value; }
        }

        public async Task StartListening(CancellationToken token)
        {
            await Task.Run(async () =>
            {

                //if (!JourneyOngoing)
                //{
                //    if (Settings.Address != "" && Settings.Longitude != "" && Settings.Latitude != "")
                //    {
                //        ChosenLocation.Longitude = Double.Parse(Settings.Longitude);
                //        ChosenLocation.Latitude = Double.Parse(Settings.Latitude);
                //        ChosenLocation.Address = Settings.Address;

                //        var locator = CrossGeolocator.Current;
                //        locator.PositionChanged += Locator_PositionChanged;

                //        if (!CrossGeolocator.Current.IsListening)
                //        {
                //            await locator.StartListeningAsync(new TimeSpan(1), .1);
                //        }
                //        JourneyOngoing = true;
                //        int timeCounter = 0;
                //        var tickedMessage = new TickedMessage
                //        {
                //            TimeTick = timeCounter
                //        };
                //        Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                //        {
                //            token.ThrowIfCancellationRequested();

                //            if (JourneyOngoing)
                //            {
                //                //send tick
                //                Device.BeginInvokeOnMainThread(() => {
                //                    MessagingCenter.Send<TickedMessage>(tickedMessage, "TickedMessage");
                //                });
                //                return true;
                //            }

                //            return false;

                //        });

                //    }
                //}
                //else
                //{
                //    CrossToastPopUp.Current.ShowToastMessage("Set the location address to continue");
                //    await Application.Current.MainPage.Navigation.PushAsync(new SearchPage());
                //}

                var tickedMessage = new TickedMessage
                {
                    TimeTick = 0
                };
                var runOnce = false;
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    if (!runOnce)
                    {
                        Device.BeginInvokeOnMainThread(() => {
                            MessagingCenter.Send<TickedMessage>(tickedMessage, "StartJourneyMessage");
                        });
                        runOnce = true;
                    }
                }


            }, token);


        }



        //private void Locator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        //{
        //    var locationMessage = new LocationMessage
        //    {
        //        ChangedLocation = e
        //    };
        //    //send e
        //    Device.BeginInvokeOnMainThread(() => {
        //        MessagingCenter.Send<LocationMessage>(locationMessage, "TickedMessage");
        //    });
        //}


    }
}
