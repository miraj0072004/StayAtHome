using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using StayAtHome.ViewModels;

namespace StayAtHome.Commands
{
    public class StartJourneyCommand:ICommand
    {
        private readonly MapViewModel _mapViewModel;

        public StartJourneyCommand(MapViewModel mapViewModel)
        {
            _mapViewModel = mapViewModel;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //_mapViewModel.StartListening();
            _mapViewModel.StartJourneyCommandExecute();
        }

        public event EventHandler CanExecuteChanged;
    }
}
