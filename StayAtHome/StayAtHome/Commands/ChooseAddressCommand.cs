using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using StayAtHome.ViewModels;

namespace StayAtHome.Commands
{
    public class ChooseAddressCommand:ICommand
    {
        private readonly SearchViewModel _searchViewModel;

        public ChooseAddressCommand(SearchViewModel searchViewModel)
        {
            _searchViewModel = searchViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _searchViewModel.SetAddressChosen();
        }

        public event EventHandler CanExecuteChanged;
    }
}
