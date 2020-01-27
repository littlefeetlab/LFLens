using System;
using System.Collections.Generic;
using System.Text;

namespace LFLens.Models
{
    public enum MenuItemType
    {
       
        Home,
        ListofPhotos,
        Settings,
        About
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
