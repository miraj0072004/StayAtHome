using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using StayAtHome.ViewModels;

namespace StayAtHome.Commands
{
    public class SearchCommand:ICommand
    {
        private readonly SearchViewModel _searchViewModel;

        public SearchCommand(SearchViewModel searchViewModel)
        {
            _searchViewModel = searchViewModel;
            
        }

        public bool CanExecute(object parameter)
        {
            //throw new NotImplementedException();
            return true;
        }

        public void Execute(object parameter)
        {
            _searchViewModel.SearchAddresses();
        }

        public event EventHandler CanExecuteChanged;
    }
}
