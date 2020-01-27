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
            if (AuthenticationState.Authenticator != null)
            {
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
                    AccessToken = Application.Current.Properties["access_token"].ToString(),

                    RefreshToken = Application.Current.Properties["refresh_token"].ToString(),
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
               
            }
          //  else { await Navigation.PushModalAsync(new NavigationPage(new OAuth())); }
           
            return service;

        }


        public static void CreateAppFolder(string FolderName)
        {

            Google.Apis.Drive.v3.DriveService service = GetDriveService();

            var FileMetaData = new Google.Apis.Drive.v3.Data.File();
            FileMetaData.MimeType = "application/vnd.google-apps.folder";
            FileMetaData.Name = FolderName;
            Google.Apis.Drive.v3.FilesResource.CreateRequest request;
            request = service.Files.Create(FileMetaData);
            request.Fields = "id";
            var file = request.Execute();

        }

        public static bool CheckFolder(string FolderName)
        {
            bool IsExist = false;

            Google.Apis.Drive.v3.DriveService service = GetDriveService();

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

        public static void CreateFolderInFolder(string folderId)
        {
            Google.Apis.Drive.v3.DriveService service = GetDriveService();

            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileName("Photos"),
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
            Application.Current.Properties.Add("PhotosFolderID", file.Id);

            var file1 = request;

        }

        public static List<GoogleDriveFiles> GetDriveFiles()
        {
            Google.Apis.Drive.v3.DriveService service = GoogleDriveFiles.GetDriveService();

            // Define parameters of request.
            Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = service.Files.List();
            FileListRequest.Fields = "nextPageToken, files(createdTime, id, name, size, version, trashed, parents)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<GoogleDriveFiles> FileList = new List<GoogleDriveFiles>();

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    GoogleDriveFiles File = new GoogleDriveFiles
                    {
                        Id = file.Id,
                        Name = file.Name,
                        Size = file.Size,
                        Version = file.Version,
                        CreatedTime = file.CreatedTime,
                        Parents = file.Parents
                    };
                    FileList.Add(File);
                }
            }
            return FileList;
        }

        public static void DeleteFile(GoogleDriveFiles files)
        {
            Google.Apis.Drive.v3.DriveService service = GoogleDriveFiles.GetDriveService();
            try
            {
                // Initial validation.    
                if (service == null)
                    throw new ArgumentNullException("service");

                if (files == null)
                    throw new ArgumentNullException(files.Id);

                // Make the request.    
                service.Files.Delete(files.Id).Execute();
            }
            catch (Exception ex)
            {
                throw new Exception("Request Files.Delete failed.", ex);
            }
        }



    }
}
