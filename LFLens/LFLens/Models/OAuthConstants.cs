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
        public static string subscriptionKey = "XXXXXXXXXXX";
        public static string uriBase = "https://XXXXXXXXXXX.api.cognitive.microsoft.com/vision/v2.0/analyze";
        // Google OAuth
        // For Google login, configure at https://console.developers.google.com/
        public static string GoogleiOSClientId = "XXXXXXXXXXX.apps.googleusercontent.com";
        public static string GoogleAndroidClientId = "XXXXXXXXXXX.apps.googleusercontent.com";
        public static string clientsecret = "XXXXXXXXXXX";

        // These values do not need changing
        public static string GoogleScope = "https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/drive.file https://www.googleapis.com/auth/drive.appfolder ";
        // public static string GoogleAuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";

        public static string GoogleAuthorizeUrl = "https://accounts.google.com/o/oauth2/v2/auth";
        public static string GoogleAccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        public static string GoogleUserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";
       

        // Set these to reversed iOS/Android client ids, with :/oauth2redirect appended
        public static string GoogleiOSRedirectUrl = "XXXXXXXXXXX:/oauth2redirect";
        public static string GoogleAndroidRedirectUrl = "com.googleusercontent.apps.XXXXXXXXXXX:/oauth2redirect";
        public static string LogFilePath = "XXXXXXXXXXX";
        public static string ConnectionString = "XXXXXX";
    }
}
