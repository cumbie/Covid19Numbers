using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Covid19Numbers.Models
{
    public class State
    {
        public State()
        {
        }

        [JsonProperty("message")]
        public string Message { get; set; }

        // list of these:
        //[
        //  {
        //    "state": "New York",
        //    "cases": 319213,
        //    "todayCases": 3991,
        //    "deaths": 24368,
        //    "todayDeaths": 299,
        //    "active": 244278,
        //    "tests": 959017,
        //    "testsPerOneMillion": 48883
        //  },

        [JsonProperty("state")]
        public string StateName { get; set; }

        [JsonProperty("cases")]
        public int Cases { get; set; }

        [JsonProperty("deaths")]
        public int Deaths { get; set; }

        [JsonProperty("todayCases")]
        public int TodayCases { get; set; }

        [JsonProperty("todayDeaths")]
        public int TodayDeaths { get; set; }

        [JsonProperty("active")]
        public int Active { get; set; }

        [JsonProperty("tests")]
        public int Tests { get; set; }

        #region Internal Values

        [JsonIgnore]
        public double TotalCountryCases { get; set; }

        [JsonIgnore]
        public double TotalCountryDeaths { get; set; }

        [JsonIgnore]
        public double TotalCountryTests { get; set; }

        [JsonIgnore]
        public int Recovered => this.Cases - this.Active;

        [JsonIgnore]
        public double PercentCases => Math.Round(100.0 * (double)this.Cases / this.TotalCountryCases, 3);

        [JsonIgnore]
        public double PercentDeaths => Math.Round(100.0 * (double)this.Deaths / this.Cases, 3);

        [JsonIgnore]
        public double PercentTotalDeaths => Math.Round(100.0 * (double)this.Deaths / this.TotalCountryDeaths, 3);

        [JsonIgnore]
        public double PercentTodayDeaths => (this.TodayCases != 0) ? Math.Round(100.0 * (double)this.TodayDeaths / this.TodayCases, 3) : 0;

        [JsonIgnore]
        public double PercentRecovered => Math.Round(100.0 * (double)this.Recovered / this.Cases, 3);

        [JsonIgnore]
        public double PercentActive => (this.Cases != 0) ? Math.Round(100.0 * (double)this.Active / this.Cases, 3) : 0;

        [JsonIgnore]
        public double PercentTests => Math.Round(100.0 * (double)this.Tests / this.TotalCountryTests, 3);

        #endregion
    }
}
