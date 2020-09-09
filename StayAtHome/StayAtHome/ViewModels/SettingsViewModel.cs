using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using StayAtHome.Annotations;
using StayAtHome.Helpers;

namespace StayAtHome.ViewModels
{
    public class SettingsViewModel: INotifyPropertyChanged
    {
        private double _distanceRestriction;
        private double _timeRestriction;

        public double TimeRestriction
        {
            get => _timeRestriction;
            set
            {
                _timeRestriction = value;
                Settings.TimeRestriction = _timeRestriction.ToString();
                OnPropertyChanged();

            }
        }

        public SettingsViewModel()
        {
            _distanceRestriction = double.Parse(Settings.DistanceRestriction);
            _timeRestriction = double.Parse(Settings.TimeRestriction);
        }

        public double DistanceRestriction
        {
            get => _distanceRestriction;
            set
            {
                _distanceRestriction = value;
                Settings.DistanceRestriction = _distanceRestriction.ToString();
                OnPropertyChanged();
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
