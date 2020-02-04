using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net.Http;
using LFLens.Models;
using Google.Apis.Drive.v3;
using LFLens.Helpers;

namespace LFLens.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OAuth : ContentPage
    {
        Account account;
        [Obsolete]
        AccountStore store;
        [Obsolete]
        public OAuth()
        {
            InitializeComponent();
            store = AccountStore.Create();
        }

        [Obsolete]
        void OnGoogleLoginClicked(object sender, EventArgs e)
        {
            string clientId = null;
            string redirectUri = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = OAuthConstants.GoogleiOSClientId;
                    redirectUri = OAuthConstants.GoogleiOSRedirectUrl;
                    break;

                case Device.Android:
                    clientId = OAuthConstants.GoogleAndroidClientId;
                    redirectUri = OAuthConstants.GoogleAndroidRedirectUrl;
                    break;
                case Device.UWP:
                    clientId = OAuthConstants.GoogleAndroidClientId;
                    redirectUri = OAuthConstants.GoogleAndroidRedirectUrl;
                    break;
            }

            account = store.FindAccountsForService(OAuthConstants.AppName).FirstOrDefault();
            var authenticator = new OAuth2Authenticator(
                 clientId,
                 null,
                 OAuthConstants.GoogleScope,
                 new Uri(OAuthConstants.GoogleAuthorizeUrl),
                 new Uri(redirectUri),
                 new Uri(OAuthConstants.GoogleAccessTokenUrl),
                 null,
                 true);


            authenticator.Completed += OnAuthCompleted;
            authenticator.Error += OnAuthError;
            AuthenticationState.Authenticator = authenticator;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);
        }
        [Obsolete]
        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }


            if (e.IsAuthenticated)
            {
                OAuthUserDetails user = null;

                // If the user is authenticated, request their basic user data from Google
                // UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
                var request = new OAuth2Request("GET", new Uri(OAuthConstants.GoogleUserInfoUrl), null, e.Account);
                var response = await request.GetResponseAsync();
                if (response != null)
                {
                    // Deserialize the data and store it in the account store
                    // The users email address will be used to identify data in SimpleDB

                    string userJson = await response.GetResponseTextAsync();
                    user = JsonConvert.DeserializeObject<OAuthUserDetails>(userJson);
                }
                LFLens.Helpers.Settings.AccessToken = null;
                LFLens.Helpers.Settings.RefreshToken = null;
                LFLens.Helpers.Settings.AccessTokenExpirationDate = DateTime.UtcNow;
                await store.SaveAsync(account = e.Account, OAuthConstants.AppName);
                if (account != null)
                {

                    //  store.Delete(account, OAuthConstants.AppName);
                    //Application.Current.Properties.Remove("access_token");
                    //Application.Current.Properties.Remove("refresh_token");
                    //Application.Current.Properties.Add("access_token", account.Properties["access_token"]);
                    // Application.Current.Properties.Add("refresh_token", account.Properties["refresh_token"]);
                    LFLens.Helpers.Settings.AccessToken = account.Properties["access_token"].ToString();
                    LFLens.Helpers.Settings.RefreshToken = account.Properties["refresh_token"].ToString();

                    LFLens.Helpers.Settings.AccessTokenExpirationDate = DateTime.UtcNow.AddSeconds(Convert.ToDouble(account.Properties["expires_in"].ToString()));
                    Google.Apis.Drive.v3.DriveService service = GoogleDriveFiles.GetDriveService();
                    bool isRootFolderExists = GoogleDriveFiles.CheckFolder(OAuthConstants.AppName, service);
                  //  bool isPhotosFolderExists = GoogleDriveFiles.CheckFolder(OAuthConstants.PhotosFolderName , service);
                   
                    if (isRootFolderExists == false)
                    { GoogleDriveFiles.CreateAppFolder(OAuthConstants.AppName, service); }


                   
                    FilesResource.ListRequest listRequest = service.Files.List();
                    IList<Google.Apis.Drive.v3.Data.File> mfiles = listRequest.Execute().Files;
                    foreach (var file in mfiles)
                    {
                        if (file.Name == OAuthConstants.AppName)
                        {
                            LFLens.Helpers.Settings.RootFolderID = file.Id;
                                                 

                        }
                        if(file.Name == OAuthConstants.PhotosFolderName)
                        {
                            LFLens.Helpers.Settings.PhotosFolderID  = file.Id;

                        }
                       
                    }

                    await Navigation.PushAsync(new ItemsPage());
                    //     
                }


                //Application.Current.Properties.Remove("Id");
                //Application.Current.Properties.Remove("FirstName");
                //Application.Current.Properties.Remove("LastName");
                //Application.Current.Properties.Remove("DisplayName");
                //Application.Current.Properties.Remove("EmailAddress");
                //Application.Current.Properties.Remove("ProfilePicture");
                LFLens.Helpers.Settings.Username = user.Name;
                LFLens.Helpers.Settings.EmailID = user.Email;
                //Application.Current.Properties.Add("Id", user.Id);
                //Application.Current.Properties.Add("FirstName", user.GivenName);
                //Application.Current.Properties.Add("LastName", user.FamilyName);
                //Application.Current.Properties.Add("DisplayName", user.Name);
                //Application.Current.Properties.Add("EmailAddress", user.Email);
                //Application.Current.Properties.Add("ProfilePicture", user.Picture);
               

            }

        }



        [Obsolete]
        void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            Debug.WriteLine("Authentication error: " + e.Message);
        }
    }
}