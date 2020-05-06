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

        //{
        //  "updated": 1588221693614,
        //  "country": "USA",
        //  "countryInfo": {
        //    "_id": 840,
        //    "iso2": "US",
        //    "iso3": "USA",
        //    "lat": 38,
        //    "long": -97,
        //    "flag": "https://corona.lmao.ninja/assets/img/flags/us.png"
        //  },
        //  "cases": 1064572,
        //  "todayCases": 378,
        //  "deaths": 61669,
        //  "todayDeaths": 13,
        //  "recovered": 147411,
        //  "active": 855492,
        //  "critical": 18671,
        //  "casesPerOneMillion": 3216,
        //  "deathsPerOneMillion": 186,
        //  "tests": 6139911,
        //  "testsPerOneMillion": 18549,
        //  "continent": "North America"
        //}

        [JsonProperty("updated")]
        public long Updated { get; set; }

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

        [JsonProperty("critical")]
        public int Critical { get; set; }

        [JsonProperty("tests")]
        public int Tests { get; set; }

        #region Internal Values

        [JsonIgnore]
        public DateTime UpdateTime => DateTimeOffset.FromUnixTimeMilliseconds(this.Updated).DateTime;

        [JsonIgnore]
        public DateTime UpdateLocalTime => TimeZoneInfo.ConvertTimeFromUtc(UpdateTime, TimeZoneInfo.Local);

        [JsonIgnore]
        public int TotalGlobalCases { get; set; }

        [JsonIgnore]
        public int TotalGlobalDeaths { get; set; }

        [JsonIgnore]
        public int TotalGlobalRecovered { get; set; }

        [JsonIgnore]
        public int TotalGlobalTests { get; set; }

        [JsonIgnore]
        public double PercentCases => Math.Round(100.0 * (double)this.Cases / this.TotalGlobalCases, 3);

        [JsonIgnore]
        public double PercentDeaths => Math.Round(100.0 * (double)this.Deaths / this.TotalGlobalDeaths, 3);

        [JsonIgnore]
        public double PercentDeathsCountry => (this.Cases != 0) ? Math.Round(100.0 * (double)this.Deaths / this.Cases, 3) : 0;

        [JsonIgnore]
        public double PercentTodayDeaths => (this.TodayCases!=0) ? Math.Round(100.0 * (double)this.TodayDeaths / this.TodayCases, 3) : 0;

        [JsonIgnore]
        public double PercentRecovered => Math.Round(100.0 * (double)this.Recovered / this.TotalGlobalRecovered, 3);

        [JsonIgnore]
        public double PercentRecoveredCountry => (this.Cases != 0) ? Math.Round(100.0 * (double)this.Recovered / this.Cases, 3) : 0;

        [JsonIgnore]
        public double PercentActive => (this.Cases!=0) ? Math.Round(100.0 * (double)this.Active / this.Cases, 3) : 0;

        [JsonIgnore]
        public double PercentCritical => (this.Cases!=0) ? Math.Round(100.0 * (double)this.Critical / this.Cases, 3) : 0;

        [JsonIgnore]
        public double PercentTests => Math.Round(100.0 * (double)this.Tests / this.TotalGlobalTests, 3);

        #endregion
    }

    public class CountryInfo
    {
        [JsonProperty("_id")]
        public string ID { get; set; }

        [JsonProperty("flag")]
        public string FlagImageUrl { get; set; }
    }
}
