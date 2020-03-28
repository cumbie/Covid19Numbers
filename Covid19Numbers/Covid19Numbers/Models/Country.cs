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

        [JsonProperty("cases")]
        public int Cases { get; set; }

        [JsonProperty("todayCases")]
        public int TodayCases { get; set; }
    }
}
