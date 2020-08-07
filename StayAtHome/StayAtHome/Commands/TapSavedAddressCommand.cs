using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using StayAtHome.ViewModels;

namespace StayAtHome.Commands
{
    public class TapSavedAddressCommand : ICommand
    {
        private readonly SavedPlacesViewModel _savedPlacesViewModel;

        public TapSavedAddressCommand(SavedPlacesViewModel savedPlacesViewModel)
        {
            _savedPlacesViewModel = savedPlacesViewModel;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _savedPlacesViewModel.SetSavedAddressChosen();
        }

        public event EventHandler CanExecuteChanged;
    }
}
