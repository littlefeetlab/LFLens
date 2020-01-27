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

namespace LFLens.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;

        Google.Apis.Drive.v3.DriveService service = GoogleDriveFiles.GetDriveService();



        public ItemsPage()
        {

            InitializeComponent();

            BindingContext = viewModel = new ItemsViewModel();
            tbBack.Clicked += (object sender, System.EventArgs e) =>
            {
                btnCapture.IsVisible = true;
                btnGallery.IsVisible = true;
              //  imgChoosen.IsVisible = false;
                imgChoosen.Source = Device.RuntimePlatform == Device.Android
                ? ImageSource.FromFile("cameraicon.jpg")
                : ImageSource.FromFile("Images/cameraicon.jpg");
                lblResult.IsVisible = false;
                clipboard.IsVisible = false;
                // Perform action
            };

        }

        [Obsolete]
        private async void GalleryBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                //if (AuthenticationState.Authenticator != null)
                //{
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
                        //{ //imgChoosen.Source = "cameraicon.png"; }
                        return;

               
                var imgSource = ImageSource.FromStream(() => { return photo.GetStream(); });
                    imgChoosen.Source = imgSource;
                imgChoosen.IsVisible = true;
                string filepath = photo.Path;
                btnCapture.IsVisible = false;
                btnGallery.IsVisible = false;

                string FileName = Path.GetFileName(filepath);
                    byte[] getByte = GetImageAsByteArray(photo.GetStream());

                    ByteArrayContent content = new ByteArrayContent(getByte);
                    string PhotosFolderID = Application.Current.Properties["PhotosFolderID"].ToString();
                    var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = string.Format("LFLens_{0}.jpg", DateTime.Now.ToString("yyyyMMdd_HHmmss")),
                        Parents = new List<string>()
    {
       PhotosFolderID
    }

                    };
              //  string FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));

                FilesResource.CreateMediaUpload request;

                    request = service.Files.Create(
                        fileMetadata, photo.GetStream(), "image/jpeg");
                    request.Fields = "id";
                    request.Upload();

                    var file = request.ResponseBody;

                    await MakeAnalysisRequest(content, fileMetadata.Name.ToString(), filepath);

                //}
                //else { await Navigation.PushModalAsync(new NavigationPage(new OAuth())); }
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
                //if (AuthenticationState.Authenticator != null)
                //{
                    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        await DisplayAlert("No Camera", ":( No camera available.", "OK");
                        return;
                    }


                    var photo = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        CustomPhotoSize = 50,
                        PhotoSize = PhotoSize.Small,
                        AllowCropping = true,
                        Directory = OAuthConstants.AppName,
                        Name = "LFLens_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                        SaveToAlbum = true



                    });
                    if (photo != null)
                    {
                    btnCapture.IsVisible = false;
                    btnGallery.IsVisible = false;
                    imgChoosen.IsVisible = true;
                    var imgSource = ImageSource.FromStream(() => { return photo.GetStream(); });
                        imgChoosen.Source = imgSource;

                        string filepath = photo.Path;


                        string FileName = Path.GetFileName(filepath);
                        byte[] getByte = GetImageAsByteArray(photo.GetStream());

                        ByteArrayContent content = new ByteArrayContent(getByte);
                        string PhotosFolderID = Application.Current.Properties["PhotosFolderID"].ToString();
                        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                        {
                            Name = FileName,
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
                        // }
                        var file = request.ResponseBody;

                        await MakeAnalysisRequest(content, FileName, filepath);

                    }
                  //  else { imgChoosen.Source = "cameraicon.png"; }
                //}
                //else { await Navigation.PushModalAsync(new NavigationPage(new OAuth())); }
            }
            catch (Exception ex)
            {
                string test = ex.Message;
                await DisplayAlert(null, ex.Message, "OK");
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

        public async Task MakeAnalysisRequest(ByteArrayContent content, string FileName, string filepath)
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
                lblResult.IsVisible = true;
                lblResult.Text = analysesResult.description.captions[0].text.ToString();
                clipboard.IsVisible = true;

                clipboard.Clicked += async (sender, args) =>
                {
                    _ = Clipboard.SetTextAsync(lblResult.Text);
                    if (Clipboard.HasText)
                    {
                        var text = await Clipboard.GetTextAsync();
                       await DisplayAlert("Success", "Copied Successfully", "OK");
                    }
                };
                ImageDetails ImgItemDetails = new ImageDetails();
                ImgItemDetails.Name = Application.Current.Properties["DisplayName"].ToString();
                ImgItemDetails.EmailID = Application.Current.Properties["EmailAddress"].ToString();
                ImgItemDetails.MobileModel = device;
                ImgItemDetails.MobilePlatform = iplatform.ToString();
                ImgItemDetails.ImageName = FileName;
                ImgItemDetails.ImageURL = filepath;
                ImgItemDetails.ImageCategory = analysesResult.tags.FirstOrDefault().name.ToString();
                //  categories.FirstOrDefault().name.ToString();
                ImgItemDetails.ImageDescription = analysesResult.description.captions[0].text.ToString();
                ImgItemDetails.IsStoreGooglePhotos = true;

                ImgItemDetails.PartitionKey = OAuthConstants.AppName;
                ImgItemDetails.RowKey = Guid.NewGuid().ToString();
                ImgItemDetails.Timestamp = DateTime.Now;
                ImgItemDetails.CreatedTime = DateTime.Now.ToLongDateString();
                //  AzureTableManager TableManagerObj = new AzureTableManager("LFSLens");
                //   TableManagerObj.InsertEntity<ImageDetails>(ImgItemDetails, true);

                //string Directory=Environment.CurrentDirectory.ToString();
                //string Dt = FileSystem.AppDataDirectory.ToString();
                string JSONFileName = string.Format("{0}.txt", DateTime.Now.ToString("yyyyMMdd"));
                string JSONFilepath = Path.Combine(OAuthConstants.LogFilePath, JSONFileName);
                string RootFolderID = Application.Current.Properties["RootFolderID"].ToString();

                string json = JsonConvert.SerializeObject(ImgItemDetails, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented, CheckAdditionalContent = true });
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = JSONFileName,
                    Parents = new List<string>()
                    {
                        RootFolderID
                    }




                };
                //  string gdFileID = string.Empty;
                string LogFileID=string.Empty;

                if (File.Exists(JSONFilepath))
                {
                   
                    Google.Apis.Drive.v3.DriveService service = GoogleDriveFiles.GetDriveService();
                    var requestFileList = service.Files.List();
                    requestFileList.Spaces = "drive";
                    requestFileList.Fields = "nextPageToken, files(id, name)";
                    requestFileList.PageSize = 10;
                    var result = requestFileList.Execute();
                    foreach (var file in result.Files)
                    {
                       // Console.WriteLine(String.Format(
                         //   "Found file: {0} ({1})", file.Name, file.Id));
                        if (file.Name == JSONFileName)
                        {
                            service.Files.Delete(file.Id).Execute();


                        }
                    }
                    if (!(File.Exists(JSONFilepath)))
                {
                    File.CreateText(JSONFilepath).Dispose();
                    File.AppendAllText(JSONFilepath, json);

                   

                }
                else
                {

                    json = string.Format(",{0}", json);
                }
               
                File.AppendAllText(JSONFilepath, json);
                    Google.Apis.Drive.v3.FilesResource.CreateMediaUpload request;
                    using (var stream = new System.IO.FileStream(JSONFilepath, System.IO.FileMode.Open))
                    {
                        request = service.Files.Create(fileMetadata, stream, fileMetadata.MimeType);
                        request.Fields = "id";
                        request.Upload();
                    }
                    var file1 = request.ResponseBody;
                   
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }

    }
}