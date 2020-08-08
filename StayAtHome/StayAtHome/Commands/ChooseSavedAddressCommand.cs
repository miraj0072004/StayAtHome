using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using StayAtHome.ViewModels;

namespace StayAtHome.Commands
{
    public class ChooseSavedAddressCommand: ICommand
    {
        private readonly SavedPlacesViewModel _savedPlacesViewModel;

        public ChooseSavedAddressCommand(SavedPlacesViewModel savedPlacesViewModel)
        {
            _savedPlacesViewModel = savedPlacesViewModel;
        }
        public bool CanExecute(object parameter)
        {
            var addressChosen = (bool?)parameter ?? false;

            if (addressChosen)
            {
                return true;
            }

            return false;
        }

        public void Execute(object parameter)
        {
            _savedPlacesViewModel.NavigateToMapPage();
        }

        public event EventHandler CanExecuteChanged;
    }
}
