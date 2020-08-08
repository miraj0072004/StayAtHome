﻿using System;
using System.Collections.Generic;
using System.Text;

namespace StayAtHome.ViewModels
{
    public class MapViewModel
    {
        private double _longitude;
        private double _latitude;

        public double Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }


        public double Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        public MapViewModel(double longitude, double latitude)
        {
            _longitude = longitude;
            _latitude = latitude;
        }
    }
}
