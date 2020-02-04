using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace LFLens
{
    [JsonObject]
    public class ImageDetails : TableEntity
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("EmailID")]
        public string EmailID { get; set; }
        public string MobileModel { get; set; }
        public string MobilePlatform { get; set; }
        [JsonProperty("ImageName")]
        public string ImageName { get; set; }
        [JsonProperty("ImageURL")]
        public string ImageURL { get; set; }
        [JsonProperty("Category")]
        public string ImageCategory { get; set; }
        [JsonProperty("Description")]
        public string ImageDescription { get; set; }

        public bool IsStoreGooglePhotos { set; get; }

        public string CreatedTime { get; set; }
        [JsonProperty("DriveFileID")]
        public string DriveFileID { get; set; }
            
        public ImageDetails()
        {
            this.RowKey = Guid.NewGuid().ToString();
        }


        public List<ImageDetails> GetImageAnalyseDetails()
        {

            string JSONFilepath = OAuthConstants.LogFilePath;
            var JsonList = new List<ImageDetails>();

            string[] files = Directory.GetFiles(JSONFilepath);
            
            foreach (var file in files)
            {
               
                string fileString = string.Format("[{0}]", File.ReadAllText(file));
                var items = JsonConvert.DeserializeObject<List<ImageDetails>>(fileString);
                foreach (var item in items)
                {
                   
                    JsonList.Add(item);

                }
               
            }
           
            return JsonList;
        }

    }
}
