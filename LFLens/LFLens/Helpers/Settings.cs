using System;
using System.Collections.Generic;
using System.Text;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace LFLens.Helpers
{
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string SettingsKey = "settings_key";
        private static readonly string SettingsDefault = string.Empty;

        #endregion


        public static string GeneralSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(SettingsKey, value);
            }
        }
        public static string Username
        {
            get
            {
                return AppSettings.GetValueOrDefault("Username", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("Username", value);
            }
        }
       
        public static string EmailID
        {
            get
            {
                return AppSettings.GetValueOrDefault("EmailID", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("EmailID", value);
            }
        }

        public static string UserProfileURL
        {
            get
            {
                return AppSettings.GetValueOrDefault("UserProfileURL", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("UserProfileURL", value);
            }
        }
        public static string AccessToken
        {
            get
            {
                return AppSettings.GetValueOrDefault("AccessToken", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("AccessToken", value);
            }
        }

        public static string RefreshToken
        {
            get
            {
                return AppSettings.GetValueOrDefault("RefreshToken", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("RefreshToken", value);
            }
        }
       
        public static string RootFolderID
        {
            get
            {
                return AppSettings.GetValueOrDefault("RootFolderID", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("RootFolderID", value);
            }
        }

        public static string PhotosFolderID
        {
            get
            {
                return AppSettings.GetValueOrDefault("PhotosFolderID", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("PhotosFolderID", value);
            }
        }

        public static string LogFileID
        {
            get
            {
                return AppSettings.GetValueOrDefault("LogFileID", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("LogFileID", value);
            }
        }
        public static DateTime AccessTokenExpirationDate
        {
            get
            {
                return AppSettings.GetValueOrDefault("AccessTokenExpirationDate", DateTime.UtcNow);
            }
            set
            {
                AppSettings.AddOrUpdateValue("AccessTokenExpirationDate", value);
            }
        }

        public static bool StoreHistory
        {
            get
            {
                return AppSettings.GetValueOrDefault("StoreHistory", false);
            }
            set
            {
                AppSettings.AddOrUpdateValue("StoreHistory", value);
            }
        }
        public static bool ShareWithLFLens
        {
            get
            {
                return AppSettings.GetValueOrDefault("ShareWithLFLens", false);
            }
            set
            {
                AppSettings.AddOrUpdateValue("ShareWithLFLens", value);
            }
        }
    }
}
