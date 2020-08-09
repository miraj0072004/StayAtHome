using System;
using System.Collections.Generic;
using System.Text;
using StayAtHome.Models;


namespace StayAtHome.ViewModels
{
    public class MapViewModel
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

        public LocalAddress ChosenLocation
        {
            get { return _chosenLocation; }
            set { _chosenLocation = value; }
        }


        public MapViewModel(LocalAddress chosenLocation)
        {
            ChosenLocation = chosenLocation;
        }
    }
}
