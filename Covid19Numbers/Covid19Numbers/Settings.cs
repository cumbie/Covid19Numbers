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
        }

        public static List<SelectCountryModel> AllCountries
        {
            get => JsonConvert.DeserializeObject<List<SelectCountryModel>>(AllCountriesJson);
            set => AllCountriesJson = JsonConvert.SerializeObject(value);
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
    }
}
