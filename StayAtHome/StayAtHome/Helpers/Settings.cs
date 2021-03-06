﻿using System;
using System.Collections.Generic;
using System.Text;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace StayAtHome.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
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

        public static string Address
        {
            get
            {
                return AppSettings.GetValueOrDefault("Address", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("Address", value);
            }
        }

        public static string Longitude
        {
            get
            {
                return AppSettings.GetValueOrDefault("Longitude", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("Longitude", value);
            }
        }

        public static string Latitude
        {
            get
            {
                return AppSettings.GetValueOrDefault("Latitude", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("Latitude", value);
            }
        }

        public static string TimeRestriction
        {
            get
            {
                return AppSettings.GetValueOrDefault("TimeRestriction", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("TimeRestriction", value);
            }
        }

        public static string DistanceRestriction
        {
            get
            {
                return AppSettings.GetValueOrDefault("DistanceRestriction", "");
            }
            set
            {
                AppSettings.AddOrUpdateValue("DistanceRestriction", value);
            }
        }



    }
}
