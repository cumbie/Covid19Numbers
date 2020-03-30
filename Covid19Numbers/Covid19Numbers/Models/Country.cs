using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Covid19Numbers.Models
{
    public class Country
    {
        public Country()
        {
            
        }

        [JsonProperty("country")]
        public string CountryName { get; set; }

        [JsonProperty("countryInfo")]
        public CountryInfo Info { get; set; }

        [JsonProperty("cases")]
        public int Cases { get; set; }

        [JsonProperty("deaths")]
        public int Deaths { get; set; }

        [JsonProperty("todayCases")]
        public int TodayCases { get; set; }

        [JsonProperty("todayDeaths")]
        public int TodayDeaths { get; set; }

        [JsonProperty("recovered")]
        public int Recovered { get; set; }

        [JsonProperty("active")]
        public int Active { get; set; }
    }

    public class CountryInfo
    {
        [JsonProperty("_id")]
        public string ID { get; set; }

        [JsonProperty("flag")]
        public string FlagImageUrl { get; set; }
    }
}
