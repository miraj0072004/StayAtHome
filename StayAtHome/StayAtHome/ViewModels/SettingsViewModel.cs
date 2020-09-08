using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using StayAtHome.Helpers;

namespace StayAtHome.ViewModels
{
    public class SettingsViewModel
    {
        private double _distanceRestriction;
        private readonly double _timeRestriction;

        public SettingsViewModel()
        {
            _distanceRestriction = double.Parse(Settings.DistanceRestriction);
            _timeRestriction = double.Parse(Settings.TimeRestriction);
        }

        public double DistanceRestriction
        {
            get => _distanceRestriction;
            set => _distanceRestriction = value;
        }

        public double TimeRestriction => _timeRestriction;
    }
}
