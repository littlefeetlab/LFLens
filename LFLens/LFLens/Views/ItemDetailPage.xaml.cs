using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using LFLens.Models;
using LFLens.ViewModels;
using Xamarin.Essentials;

namespace LFLens.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel;

        public ItemDetailPage()
        {
            InitializeComponent();
            viewModel = new ItemDetailViewModel();
            BindingContext = viewModel;

        }
        public void btnshare(Object Sender, System.EventArgs e)
        {
            var ShareButton = (Button)Sender;

            var ShareDetails = ShareButton.CommandParameter as ImageDetails;

           if (ShareDetails != null)

           {
             Xamarin.Essentials.Share.RequestAsync(new ShareTextRequest
            {
                Title = "Share from LFLens",
                Text = ShareDetails.ImageDescription ,
                Subject = "LFLens",
                Uri = string.Format("{0} File Sharing URL: https://drive.google.com/file/d/{1}/", Environment.NewLine, ShareDetails.DriveFileID)
            });
    }
        }

      
    }
}