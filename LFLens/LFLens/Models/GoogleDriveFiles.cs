using System;
using System.Collections.Generic;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Xamarin.Forms;
using System.Linq;
using System.IO;
using LFLens.Views;
using LFLens.Helpers;

namespace LFLens.Models
{
    class GoogleDriveFiles
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long? Size { get; set; }
        public long? Version { get; set; }
        public DateTime? CreatedTime { get; set; }
        public IList<string> Parents { get; set; }
        public static string content { get; private set; }

       
        public static Google.Apis.Drive.v3.DriveService GetDriveService()
        {
            Google.Apis.Drive.v3.DriveService service = new Google.Apis.Drive.v3.DriveService();
           
                Google.Apis.Auth.OAuth2.Flows.GoogleAuthorizationCodeFlow googleAuthFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer()
                {
                    ClientSecrets = new ClientSecrets()
                    {
                        ClientId = OAuthConstants.GoogleAndroidClientId,
                        ClientSecret = null,
                    }
                });

                Google.Apis.Auth.OAuth2.Responses.TokenResponse responseToken = new TokenResponse()
                {
                    AccessToken = LFLens.Helpers.Settings.AccessToken,

                    RefreshToken = LFLens.Helpers.Settings.RefreshToken,
                    Scope = OAuthConstants.GoogleScope,
                    TokenType = "Bearer",
                };

                var credential = new UserCredential(googleAuthFlow, "", responseToken);



                //Once consent is recieved, your token will be stored locally on the AppData directory, so that next time you wont be prompted for consent.   

                service = new Google.Apis.Drive.v3.DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = OAuthConstants.AppName,
                });
                service.HttpClient.Timeout = TimeSpan.FromMinutes(100);
               
           
          //  else { await Navigation.PushModalAsync(new NavigationPage(new OAuth())); }
           
            return service;

        }

        public static void CreateAppFolder(string FolderName, Google.Apis.Drive.v3.DriveService service )
        {

//            Google.Apis.Drive.v3.DriveService service = GetDriveService();

            var FileMetaData = new Google.Apis.Drive.v3.Data.File();
            FileMetaData.MimeType = "application/vnd.google-apps.folder";
            FileMetaData.Name = FolderName;
            Google.Apis.Drive.v3.FilesResource.CreateRequest request;
            request = service.Files.Create(FileMetaData);
            request.Fields = "id";
            var file = request.Execute();
            GoogleDriveFiles.CreateFolderInFolder(file.Id, service);

        }

        public static bool CheckFolder(string FolderName, Google.Apis.Drive.v3.DriveService service)
        {
            bool IsExist = false;

           // Google.Apis.Drive.v3.DriveService service = GetDriveService();

            // Define the parameters of the request.    
            Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = service.Files.List();
            FileListRequest.Fields = "nextPageToken, files(*)";

            // List files.    
            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<GoogleDriveFiles> FileList = new List<GoogleDriveFiles>();


            //For getting only folders    
            files = files.Where(x => x.MimeType == "application/vnd.google-apps.folder" && x.Name == FolderName).ToList();

            if (files.Count > 0)
            {
                IsExist = true;
            }
            return IsExist;
        }

        public static void CreateFolderInFolder(string folderId, Google.Apis.Drive.v3.DriveService service)
        {
           // Google.Apis.Drive.v3.DriveService service = GetDriveService();

            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileName(OAuthConstants.PhotosFolderName),
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string>
                   {
                       folderId
                   }
            };


            Google.Apis.Drive.v3.FilesResource.CreateRequest request;

            request = service.Files.Create(FileMetaData);
            request.Fields = "id";
            var file = request.Execute();
            LFLens.Helpers.Settings.PhotosFolderID = file.Id;
         
            var file1 = request;

        }

     

    }
}
