using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LFLens.Models;
using LFLens.Views;
using LFLens.ViewModels;
using Plugin.Media;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using Xamarin.Essentials;
using Google.Apis.Drive.v3;

using LFLens.Helpers;

namespace LFLens.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        // ItemsViewModel viewModel;


        public string strDriveFileID = string.Empty;
        private MediaFile photo;

        public ItemsPage()
        {

            InitializeComponent();
            this.activityMonitor.BindingContext = this;
            this.IsBusy = false;

           
            Back.Clicked += (object sender, System.EventArgs e) =>
            {
                
                slResult.IsVisible = false;
                sldefault.IsVisible = true;
                this.IsBusy = false;
               
            };



        }

        [Obsolete]
        private async void GalleryBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                    return;
                }
                var photo = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small,
                    SaveMetaData = true,
                    RotateImage = true
                });


                if (photo == null)
                   return;


                var imgSource = ImageSource.FromStream(() => { return photo.GetStream(); });
                imgChoosen.Source = imgSource;

                string filepath = photo.Path;
                this.IsBusy = true;


                string FileName = Path.GetFileName(filepath);
                byte[] getByte = GetImageAsByteArray(photo.GetStream());

                ByteArrayContent content = new ByteArrayContent(getByte);
                if (LFLens.Helpers.Settings.StoreHistory == true)
                {
                    Google.Apis.Drive.v3.DriveService service = GoogleDriveFiles.GetDriveService();
                    string PhotosFolderID = LFLens.Helpers.Settings.PhotosFolderID;
                    var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = string.Format("LFLens_{0}.jpg", DateTime.Now.ToString("yyyyMMdd_HHmmss")),
                        Parents = new List<string>()
    {
       PhotosFolderID
    }

                    };
                  
                    FilesResource.CreateMediaUpload request;


                    request = service.Files.Create(
                            fileMetadata, photo.GetStream(), "image/jpeg");
                    request.Fields = "id";
                    request.Upload();

                    var driveFile = request.ResponseBody;
                    strDriveFileID = driveFile.Id;
                }
                await MakeAnalysisRequest(content, FileName, filepath, strDriveFileID);

            }
            catch (Exception ex)
            {
                string test = ex.Message;
                await DisplayAlert(null, ex.Message, "OK");
            }
        }

        [Obsolete]
        private async void CaptureBtn_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
               
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", ":( No camera available.", "OK");
                    return;
                }

                if (LFLens.Helpers.Settings.StoreHistory == true)
                {
                    photo = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        CustomPhotoSize = 50,
                        PhotoSize = PhotoSize.Small,
                        AllowCropping = true,
                        Directory = OAuthConstants.AppName,
                        Name = "LFLens_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                        SaveToAlbum = true



                    });
                }
                else
                {
                    photo = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        CustomPhotoSize = 50,
                        PhotoSize = PhotoSize.Small,
                        AllowCropping = true,
                       
                        Name = "LFLens_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                      
                    });

                }
                if (photo != null)
                {

                    var imgSource = ImageSource.FromStream(() => { return photo.GetStream(); });
                    imgChoosen.Source = imgSource;
                    
                    string filepath = photo.Path;
                    this.IsBusy = true;

                    string FileName = Path.GetFileName(filepath);
                    byte[] getByte = GetImageAsByteArray(photo.GetStream());

                    ByteArrayContent content = new ByteArrayContent(getByte);
                    if (LFLens.Helpers.Settings.StoreHistory == true)
                    {
                        string PhotosFolderID = LFLens.Helpers.Settings.PhotosFolderID;
                        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                        {
                            Name = FileName,
                            Parents = new List<string>()
    {
        PhotosFolderID
    }

                        };



                        FilesResource.CreateMediaUpload request;
                        
                        Google.Apis.Drive.v3.DriveService service = GoogleDriveFiles.GetDriveService();

                        request = service.Files.Create(
                                    fileMetadata, photo.GetStream(), "image/jpeg");
                        request.Fields = "id";
                        request.Upload();
                        var driveFile = request.ResponseBody;
                        strDriveFileID = driveFile.Id;
                    }
                    await MakeAnalysisRequest(content, FileName, filepath, strDriveFileID);

                }
              
            }
            catch (Exception ex)
            {
              
            }
        }



        public static byte[] GetImageAsByteArray(Stream Camerafile)
        {
            BinaryReader binaryReader = new BinaryReader(Camerafile);
            return binaryReader.ReadBytes((Int32)Camerafile.Length);
        }

        private byte[] ImageFileToByteArray(string path)
        {
            FileStream fs = File.OpenRead(path);
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            return bytes;
        }



        public async Task MakeAnalysisRequest(ByteArrayContent content, string FileName, string filepath, string DriveFileID)
        {
            try
            {
                HttpClient client = new HttpClient();

                // Request headers.  
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", OAuthConstants.subscriptionKey);

                string requestParameters =
                    "visualFeatures=Categories,Color,Description,Faces,Tags&language=en";

                // Assemble the URI for the REST API method.  
                string uri = OAuthConstants.uriBase + "?" + requestParameters;

                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                // Asynchronously call the REST API method.  
                HttpResponseMessage response = await client.PostAsync(uri, content);

                // Asynchronously get the JSON response.  

                string contentString = await response.Content.ReadAsStringAsync();
                //  string lpath = 


                var analysesResult = JsonConvert.DeserializeObject<Item>(contentString);
                var device = DeviceInfo.Model;
                var iplatform = DeviceInfo.Platform;

                lblResult.Text = analysesResult.description.captions[0].text.ToString();


                ImageDetails ImgItemDetails = new ImageDetails();
                ImgItemDetails.Name = LFLens.Helpers.Settings.Username;
                ImgItemDetails.EmailID = LFLens.Helpers.Settings.EmailID;
                ImgItemDetails.MobileModel = device;
                ImgItemDetails.MobilePlatform = iplatform.ToString();
                ImgItemDetails.ImageName = FileName;
                ImgItemDetails.ImageURL = filepath;
                ImgItemDetails.ImageCategory = analysesResult.tags.FirstOrDefault().name.ToString();
               
                ImgItemDetails.ImageDescription = analysesResult.description.captions[0].text.ToString();
                ImgItemDetails.IsStoreGooglePhotos = true;
                ImgItemDetails.DriveFileID = DriveFileID;
                ImgItemDetails.PartitionKey = OAuthConstants.AppName;
                ImgItemDetails.RowKey = Guid.NewGuid().ToString();
                ImgItemDetails.Timestamp = DateTime.Now;
                ImgItemDetails.CreatedTime = DateTime.Now.ToLongDateString();
                if (LFLens.Helpers.Settings.ShareWithLFLens == true)
                {
                    AzureTableManager TableManagerObj = new AzureTableManager(OAuthConstants.AppName);
                    TableManagerObj.InsertEntity<ImageDetails>(ImgItemDetails, true);
                }
                if (LFLens.Helpers.Settings.StoreHistory == true)
                {
                   
                    string JSONFileName = string.Format("{0}.txt", DateTime.Now.ToString("yyyyMMdd"));
                    string JSONFilepath = Path.Combine(OAuthConstants.LogFilePath, JSONFileName);
                    string RootFolderID = LFLens.Helpers.Settings.RootFolderID;

                    string json = JsonConvert.SerializeObject(ImgItemDetails, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented, CheckAdditionalContent = true });
                    var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = JSONFileName,
                        Parents = new List<string>()
                    {
                        RootFolderID
                    },
                        MimeType = "text/plain",




                    };
                  
                    string LogFileID = string.Empty;

                    

                    if (!(File.Exists(JSONFilepath)))
                    {
                        File.CreateText(JSONFilepath).Dispose();
                        File.AppendAllText(JSONFilepath, json);

                    }
                    else
                    {

                        json = string.Format(",{0}", json);
                        File.AppendAllText(JSONFilepath, json);
                       
                        Google.Apis.Drive.v3.DriveService service = GoogleDriveFiles.GetDriveService();
                        FilesResource.ListRequest listRequest = service.Files.List();
                        IList<Google.Apis.Drive.v3.Data.File> mfiles = listRequest.Execute().Files;
                        foreach (var logfile in mfiles)
                        {
                            if (logfile.Name == JSONFileName)
                            {
                                service.Files.Delete(logfile.Id).Execute();

                            }
                        }
                        

                    }

                    Google.Apis.Drive.v3.DriveService service1 = GoogleDriveFiles.GetDriveService();
                    FilesResource.CreateMediaUpload request;
                    using (var stream = new FileStream(JSONFilepath, System.IO.FileMode.Open))
                    {
                        request = service1.Files.Create(fileMetadata, stream, fileMetadata.MimeType);
                        request.Fields = "id";
                        request.Upload();
                    }
                    var file = request.ResponseBody;
                    btnShare.Clicked += async (sender, args) =>
                    {
                        await Xamarin.Essentials.Share.RequestAsync(new ShareTextRequest
                        {
                            Title = "Share from LFLens",
                            Text = ImgItemDetails.ImageDescription,
                            Subject = "LFLens",
                            Uri = string.Format("{0} File Sharing URL: https://drive.google.com/file/d/{1}/", Environment.NewLine, DriveFileID)
                        });
                    };
                }
                btnShare.Clicked += async (sender, args) =>
                {
                    await Xamarin.Essentials.Share.RequestAsync(new ShareTextRequest
                    {
                        Title = "Share from LFLens",
                        Text = ImgItemDetails.ImageDescription,
                        Subject = "LFLens",
                     
                    });
                };
               
                sldefault.IsVisible = false;
                slResult.IsVisible = true;


            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }

        [Obsolete]
        private async void LogoutMenuItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new OAuth());
            LFLens.Helpers.Settings.AccessToken = null;
            LFLens.Helpers.Settings.AccessTokenExpirationDate = DateTime.UtcNow;

        }

    }
}