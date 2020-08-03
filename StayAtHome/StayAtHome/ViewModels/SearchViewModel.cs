using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using StayAtHome.Commands;
using StayAtHome.Models;

namespace StayAtHome.ViewModels
{
    public class SearchViewModel :INotifyPropertyChanged
    {

        public SearchViewModel()
        {
            SearchCommand = new SearchCommand(this);
            Addresses = new List<string>();
        }

        private string _searchTerm;
        private List<string> _addresses;

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged();
            }
        }

        public List<string> Addresses
        {
            get => _addresses;
            set
            {
                _addresses = value;
                OnPropertyChanged();
            }
        }

        public SearchCommand SearchCommand { get; set; }

        public async void SearchAddresses()
        {
            if (SearchTerm.Length > 5)
            {
                Addresses = await Address.GetAddresses(SearchTerm);
            }
            else
            {
                Addresses = null;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
