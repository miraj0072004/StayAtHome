using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StayAtHome.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StayAtHome
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        public SearchPage()
        {
            InitializeComponent();
            //BindingContext = new SearchViewModel();
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SavePlacePage {BindingContext = this.searchViewModel});
        }
    }
}