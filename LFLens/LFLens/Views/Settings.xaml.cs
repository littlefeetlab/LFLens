using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LFLens.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LFLens.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();
         
            StoreDetails.IsToggled = Helpers.Settings.StoreHistory;
            ShareLFLens.IsToggled = Helpers.Settings.ShareWithLFLens;
           
        }
   
       

        private void StoreDetails_Toggled(object sender, ToggledEventArgs e)
        {

            bool isValue = e.Value;
            Helpers.Settings.StoreHistory = isValue;

        }

        private void ShareLFLens_Toggled(object sender, ToggledEventArgs e)
        {
            bool isValue = e.Value;
            Helpers.Settings.ShareWithLFLens = isValue;

        }
    }
}