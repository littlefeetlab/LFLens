using System;
using System.Collections.Generic;
using LFLens.Models;

namespace LFLens.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public List<ImageDetails> ImageListItem { get; set; }
        public ItemDetailViewModel()
        {
            ImageListItem = new ImageDetails().GetImageAnalyseDetails();

        }
    }
}
