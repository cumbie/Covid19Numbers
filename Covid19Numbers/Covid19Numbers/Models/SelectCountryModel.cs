using System;
using Newtonsoft.Json;

namespace Covid19Numbers.Models
{
    public class SelectCountryModel
    {
        public SelectCountryModel()
        {
        }

        [JsonProperty("country")]
        public string CountryName { get; set; }

        [JsonIgnore]
        public string CountryCode => this.CountryInfo?.CountryCode;

        [JsonProperty("countryInfo")]
        public SelectCountryInfo CountryInfo { get; set; }
    }

    public class SelectCountryInfo
    {
        [JsonProperty("iso3")]
        public string CountryCode { get; set; }
    }
}
