using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using SQLite;
using StayAtHome.Annotations;
using StayAtHome.Commands;
using StayAtHome.Models;

namespace StayAtHome.ViewModels
{
    public class SavedPlacesViewModel:INotifyPropertyChanged
    {
        #region Fields and Properties
        private List<LocalAddress> _savedAddresses;
        private string _selectedItem;
        private bool _addressChosen;

        public bool AddressChosen
        {
            get { return _addressChosen; }
            set
            {
                _addressChosen = value;
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


        public List<LocalAddress> SavedAddresses
        {
            get { return _savedAddresses; }
            set { _savedAddresses = value; }
        }

        public ChooseSavedAddressCommand ChooseSavedAddressCommand { get; set; }
        public TapSavedAddressCommand TapSavedAddressCommand { get; set; }
        #endregion

        public SavedPlacesViewModel()
        {
            AddressChosen = false;
            ChooseSavedAddressCommand = new ChooseSavedAddressCommand(this);
            TapSavedAddressCommand = new TapSavedAddressCommand(this);
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<LocalAddress>();
                SavedAddresses = conn.Table<LocalAddress>().ToList();
            }
        }

        public void SetSavedAddressChosen()
        {
            if (!AddressChosen)
            {
                AddressChosen = true;
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
