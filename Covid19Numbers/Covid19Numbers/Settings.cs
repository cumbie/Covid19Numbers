using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using Covid19Numbers.Models;

namespace Covid19Numbers
{
    public static class Settings
    {
        private static ISettings AppSettings => CrossSettings.Current;

        class SettingsDefaults
        {
            public static string MyCountryCode = "USA";
            public static List<string> AllCountries = new List<string>
            {
                "[ {\"country\": \"USA\", \"countryInfo\": {\"iso3\": \"USA\"}} ]"
            };
            public static int RefreshRate = 2;
        }

        /// <summary>
        /// The countries are stored as a raw JSON string. This is marked as private so we can abstract the json conversion
        /// out and just let access be from theAllCountries list.
        /// </summary>
        private static string AllCountriesJson
        {
            get => AppSettings.GetValueOrDefault(nameof(AllCountriesJson), JsonConvert.SerializeObject(SettingsDefaults.AllCountries));
            set => AppSettings.AddOrUpdateValue(nameof(AllCountriesJson), value);
        }

        /// <summary>
        /// Saved country for main view
        /// </summary>
        public static string MyCountryCode
        {
            get => AppSettings.GetValueOrDefault(nameof(MyCountryCode), SettingsDefaults.MyCountryCode);
            set => AppSettings.AddOrUpdateValue(nameof(MyCountryCode), value);
        }

        /// <summary>
        /// API call Refresh Rate
        /// </summary>
        public static int RefreshRate
        {
            get => AppSettings.GetValueOrDefault(nameof(RefreshRate), SettingsDefaults.RefreshRate);
            set => AppSettings.AddOrUpdateValue(nameof(RefreshRate), value);
        }

        ///// <summary>
        ///// last page absolute path
        ///// </summary>
        //public static string LastPagePath
        //{
        //    get => AppSettings.GetValueOrDefault(nameof(LastPagePath), string.Empty);
        //    set => AppSettings.AddOrUpdateValue(nameof(LastPagePath), value);
        //}

        ///// <summary>
        ///// current page absolute path
        ///// </summary>
        //public static string CurrentPagePath
        //{
        //    get => AppSettings.GetValueOrDefault(nameof(CurrentPagePath), string.Empty);
        //    set => AppSettings.AddOrUpdateValue(nameof(CurrentPagePath), value);
        //}
    }
}
