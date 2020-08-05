using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using StayAtHome.ViewModels;

namespace StayAtHome.Commands
{
    public class GetAddressDetailsCommand : ICommand
    {
        private readonly SearchViewModel _searchViewModel;

        public GetAddressDetailsCommand(SearchViewModel searchViewModel)
        {
            _searchViewModel = searchViewModel;
        }

        public bool CanExecute(object parameter)
        {
            var addressChosen =  (bool?) parameter ?? false;

            if (addressChosen)
            {
                return true;
            }

            return false;

        }

        public void Execute(object parameter)
        {
            _searchViewModel.SearchAddress();
        }

        public event EventHandler CanExecuteChanged;
    }
}
