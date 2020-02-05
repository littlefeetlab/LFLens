using LFLens.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LFLens.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;

        [Obsolete]
        public MenuPage()
        {
            InitializeComponent();
            bool isStoreHistory = LFLens.Helpers.Settings.StoreHistory;
            lblUserName.Text = string.Format("Welcome {0} ,", LFLens.Helpers.Settings.Username);
            string strProfileURL = LFLens.Helpers.Settings.UserProfileURL;
            if (!string.IsNullOrEmpty(strProfileURL))
            {
                UserProfile.Source = strProfileURL;
            }
            else { UserProfile.IsVisible = false; }
            if (isStoreHistory == true)
            {
                menuItems = new List<HomeMenuItem>
            {
               
                new HomeMenuItem {Id = MenuItemType.Home, Title="Home" },
                new HomeMenuItem {Id = MenuItemType.ListofPhotos, Title="List of Photos" },
                new HomeMenuItem {Id = MenuItemType.Settings, Title="Settings" },
                new HomeMenuItem {Id = MenuItemType.About, Title="About"}
            };

            }
            else 
            {
                menuItems = new List<HomeMenuItem>
            {
                
                new HomeMenuItem {Id = MenuItemType.Home, Title="Home" },
               // new HomeMenuItem {Id = MenuItemType.ListofPhotos, Title="List of Photos" },
                new HomeMenuItem {Id = MenuItemType.Settings, Title="Settings" },
                new HomeMenuItem {Id = MenuItemType.About, Title="About"}
            };
            }
            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                await RootPage.NavigateFromMenu(id);
            };
        }
    }
}