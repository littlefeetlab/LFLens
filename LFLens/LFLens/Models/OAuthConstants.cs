using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;

namespace LFLens
{
    class OAuthConstants
    {
        public static string AppName = "LFLens";
        public static string subscriptionKey = "401774cb1aa74f8f92c325a4a3718d65";
        public static string uriBase = "https://centralus.api.cognitive.microsoft.com/vision/v2.0/analyze";
        // Google OAuth
        // For Google login, configure at https://console.developers.google.com/
        public static string GoogleiOSClientId = "637158082165-qk17t56c90j8ufetku7sqh05at0s6k49.apps.googleusercontent.com";
        public static string GoogleAndroidClientId = "637158082165-qcb6res1olhm9vhn9i698g6jn5t9i0is.apps.googleusercontent.com";
        public static string clientsecret = "fAqb5rbvTxonjbOdilinAONx";

        // These values do not need changing
        public static string GoogleScope = "https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/drive.file https://www.googleapis.com/auth/drive.appfolder ";
        // public static string GoogleAuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";

        public static string GoogleAuthorizeUrl = "https://accounts.google.com/o/oauth2/v2/auth";
        public static string GoogleAccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        public static string GoogleUserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";
        public static string GooglePhotosLibrary = "https://photoslibrary.googleapis.com/v1/uploads";

        // Set these to reversed iOS/Android client ids, with :/oauth2redirect appended
        public static string GoogleiOSRedirectUrl = "637158082165-qk17t56c90j8ufetku7sqh05at0s6k49:/oauth2redirect";
        public static string GoogleAndroidRedirectUrl = "com.googleusercontent.apps.637158082165-qcb6res1olhm9vhn9i698g6jn5t9i0is:/oauth2redirect";
        public static string LogFilePath = "/storage/emulated/0/Android/data/com.companyname.lflens/files/";
    }
}
