using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LFLens.Services;
using LFLens.Views;
using LFLens.Helpers;

namespace LFLens
{
    public partial class App : Application
    {
        //TODO: Replace with *.azurewebsites.net url after deploying backend to Azure
        //To debug on Android emulators run the web backend against .NET Core not IIS
        //If using other emulators besides stock Google images you may need to adjust the IP address
       // public static string AzureBackendUrl =
         //   DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000" : "http://localhost:5000";
     //   public static bool UseMockDataStore = true;

        [Obsolete]
        public App()
        {
            InitializeComponent();

            //if (UseMockDataStore)
            //    DependencyService.Register<MockDataStore>();
            //else
            //    DependencyService.Register<AzureDataStore>();
            
           if (!string.IsNullOrEmpty(LFLens.Helpers.Settings.AccessToken) && LFLens.Helpers.Settings.AccessTokenExpirationDate < DateTime.UtcNow.AddHours(1)) 
            {
              
                    MainPage = new MainPage();
               
               
            }
            else { MainPage = new NavigationPage(new OAuth()); }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
