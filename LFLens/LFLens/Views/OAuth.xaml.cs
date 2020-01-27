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

                if (account != null)
                {
                    //  store.Delete(account, OAuthConstants.AppName);
                    Application.Current.Properties.Remove("access_token");
                    Application.Current.Properties.Remove("refresh_token");
                    Application.Current.Properties.Add("access_token", account.Properties["access_token"]);
                    Application.Current.Properties.Add("refresh_token", account.Properties["refresh_token"]);
                    bool isRootFolderExists = GoogleDriveFiles.CheckFolder(OAuthConstants.AppName);
                    bool isPhotosFolderExists = GoogleDriveFiles.CheckFolder("Photos");
                    if (isRootFolderExists == false)
                    { GoogleDriveFiles.CreateAppFolder(OAuthConstants.AppName); }

                    if (isPhotosFolderExists == false)
                    {
                        Google.Apis.Drive.v3.DriveService service = GoogleDriveFiles.GetDriveService();
                        FilesResource.ListRequest listRequest = service.Files.List();
                        IList<Google.Apis.Drive.v3.Data.File> mfiles = listRequest.Execute().Files;
                        foreach (var file in mfiles)
                        {
                            if (file.Name == OAuthConstants.AppName)
                            {
                                GoogleDriveFiles.CreateFolderInFolder(file.Id);
                                Application.Current.Properties.Add("RootFolderID", file.Id);

                            }
                        }
                    }


                    //     
                }

                await store.SaveAsync(account = e.Account, OAuthConstants.AppName);
                Application.Current.Properties.Remove("Id");
                Application.Current.Properties.Remove("FirstName");
                Application.Current.Properties.Remove("LastName");
                Application.Current.Properties.Remove("DisplayName");
                Application.Current.Properties.Remove("EmailAddress");
                Application.Current.Properties.Remove("ProfilePicture");

                Application.Current.Properties.Add("Id", user.Id);
                Application.Current.Properties.Add("FirstName", user.GivenName);
                Application.Current.Properties.Add("LastName", user.FamilyName);
                Application.Current.Properties.Add("DisplayName", user.Name);
                Application.Current.Properties.Add("EmailAddress", user.Email);
                Application.Current.Properties.Add("ProfilePicture", user.Picture);
                await Navigation.PushAsync(new ItemsPage());

            }
            else { }
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