using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using StayAtHome.ViewModels;

namespace StayAtHome.Commands
{
    public class SaveAddressCommand:ICommand
    {
        private readonly SearchViewModel _searchViewModel;

        public SaveAddressCommand(SearchViewModel searchViewModel)
        {
            _searchViewModel = searchViewModel;
        }
        public bool CanExecute(object parameter)
        {
            //uncomment if needs to be used for multiple address saving
            //var nameLength = (int?)parameter ?? 0;

            //if (nameLength == 0)
            //{
            //    return false;
            //}

            return true;
        }

        public void Execute(object parameter)
        {
            _searchViewModel.SaveAddress();
        }

        public event EventHandler CanExecuteChanged;
    }
}
