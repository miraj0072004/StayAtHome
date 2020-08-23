using System;
using System.Collections.Generic;
using System.Text;
using Plugin.Geolocator.Abstractions;

namespace StayAtHome.Messages
{
    public class LocationMessage
    {
        public PositionEventArgs ChangedLocation { get; set; }
    }
}
