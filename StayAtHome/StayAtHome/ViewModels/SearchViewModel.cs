﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Plugin.Toast;
using Plugin.Toast.Abstractions;
using SQLite;
using StayAtHome.Commands;
using StayAtHome.Helpers;
using StayAtHome.Models;
using Xamarin.Forms;

namespace StayAtHome.ViewModels
{
    public class SearchViewModel :INotifyPropertyChanged
    {

        public SearchViewModel()
        {
            SearchCommand = new SearchCommand(this);
            ChooseAddressCommand = new ChooseAddressCommand(this);
            AddressChosen = false;
            GetAddressDetailsCommand = new GetAddressDetailsCommand(this);
            SaveAddressCommand = new SaveAddressCommand(this);
            Addresses = new List<string>();
            
        }

        #region Properties and Commands
        private string _searchTerm;
        private List<string> _addresses;
        private bool _addressChosen;
        private string _selectedItem;
        private Address _address;
        private string _placeName;

        public string PlaceName
        {
            get { return _placeName; }
            set
            {
                _placeName = value;
                OnPropertyChanged();
            }
        }



        public Address Address
        {
            get { return _address; }
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }


        public string SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }




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

        public bool AddressChosen
        {
            get { return _addressChosen; }
            set
            {
                _addressChosen = value;
                OnPropertyChanged();
            }
        }

        
        public SearchCommand SearchCommand { get; set; }
        public ChooseAddressCommand ChooseAddressCommand { get; set; }
        public GetAddressDetailsCommand GetAddressDetailsCommand { get; set; }
        public SaveAddressCommand SaveAddressCommand { get; set; }
        #endregion

        public async void SearchAddresses()
        {
            if (AddressChosen)
            {
                AddressChosen = false;
            }

            if (SearchTerm.Length > 5)
            {
                Addresses = await Address.GetAddresses(SearchTerm);
            }
            else
            {
                Addresses = null;
                AddressChosen = false;
            }
        }

        public async void SearchAddress()
        {
            Address = await Address.GetAddress(SelectedItem);
        }

        public void SetAddressChosen()
        {
            if (!AddressChosen)
            {
                AddressChosen = true;
                SearchAddress();
            }
        }

        public void SaveAddress()
        {
            //uncomment if needs to be used for multiple address saving
            //LocalAddress localAddress = new LocalAddress
            //{
            //    Address = SelectedItem,
            //    Name = PlaceName,
            //    Longitude = Address.Longitude,
            //    Latitude = Address.Latitude
            //};

            //using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            //{
            //    conn.CreateTable<LocalAddress>();
            //    int rows = conn.Insert(localAddress);

            //    if (rows > 0)
            //    {
            //        //Application.Current.MainPage.DisplayAlert("Success", "Address added", "Ok");
            //        Application.Current.MainPage.Navigation.PushAsync(new SavedPlacesPage());

            //    }
            //    else
            //    {
            //        Application.Current.MainPage.DisplayAlert("Failure", "Saving address failed", "Ok");
            //    }
            //}

            Settings.Address = SelectedItem;
            Settings.Longitude = Address.Longitude.ToString();
            Settings.Latitude = Address.Latitude.ToString();

            CrossToastPopUp.Current.ShowToastMessage("Location Added Successfully", ToastLength.Long);

            Application.Current.MainPage.Navigation.PopAsync();


        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
