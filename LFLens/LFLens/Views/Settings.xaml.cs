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
            lblStoreDetails.Text = string.Format("Share text to App Developer:{0}(Only the description, Pictures are private)", Environment.NewLine);
            lblShare.Text = "Store History in your Google drive and local";
            StoreDetails.IsToggled = Helpers.Settings.StoreHistory;
            ShareLFLens.IsToggled = Helpers.Settings.ShareWithLFLens;
            //Helpers.Settings.StoreHistory = storeHistory.On;
            //Helpers.Settings.ShareWithLFLens = shareLFLens.On;
        }
     //   public event EventHandler<Xamarin.Forms.ToggledEventArgs> OnChanged;

        //private void storeHistory_Tapped(object sender, EventArgs e)
        //{
           
        //    Helpers.Settings.StoreHistory = storeHistory.IsSet;
           
        //}
       

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