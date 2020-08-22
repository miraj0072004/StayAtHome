using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Plugin.Geolocator;
using Plugin.Toast;
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

        public async void StartListening(CancellationToken token)
        {

            if (!JourneyOngoing)
            {
                if (Settings.Address != "" && Settings.Longitude != "" && Settings.Latitude != "")
                {
                    ChosenLocation.Longitude = Double.Parse(Settings.Longitude);
                    ChosenLocation.Latitude = Double.Parse(Settings.Latitude);
                    ChosenLocation.Address = Settings.Address;

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
                            //send tick
                            return true;
                        }

                        return false;

                    });

                }
            }
            else
            {
                CrossToastPopUp.Current.ShowToastMessage("Set the location address to continue");
                await Application.Current.MainPage.Navigation.PushAsync(new SearchPage());
            }
            

        }

        


        private void Locator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            //send e
        }


    }
}
