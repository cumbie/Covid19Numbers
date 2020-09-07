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

        //"state": "California",
        //"updated": 1599491574726,
        //"cases": 739154,
        //"todayCases": 0,
        //"deaths": 13730,
        //"todayDeaths": 0,
        //"recovered": 351249,
        //"active": 374175,
        //"casesPerOneMillion": 18707,
        //"deathsPerOneMillion": 347,
        //"tests": 12047191,
        //"testsPerOneMillion": 304898,
        //"population": 39512223

        [JsonProperty("updated")]
        public long Updated { get; set; }

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

        [JsonProperty("recovered")]
        public int Recovered { get; set; }

        [JsonProperty("active")]
        public int Active { get; set; }

        [JsonProperty("casesPerOneMillion")]
        public int CasesPerOneMillion { get; set; }

        [JsonProperty("deathsPerOneMillion")]
        public int DeathsPerOneMillion { get; set; }

        [JsonProperty("tests")]
        public int Tests { get; set; }

        [JsonProperty("testsPerOneMillion")]
        public int TestsPerOneMillion { get; set; }

        [JsonProperty("population")]
        public int Population { get; set; }

        #region Internal Values

        [JsonIgnore]
        public DateTime UpdateTime => DateTimeOffset.FromUnixTimeMilliseconds(this.Updated).DateTime;

        [JsonIgnore]
        public DateTime UpdateLocalTime => TimeZoneInfo.ConvertTimeFromUtc(UpdateTime, TimeZoneInfo.Local);

        [JsonIgnore]
        public double TotalCountryCases { get; set; }

        [JsonIgnore]
        public double TotalCountryDeaths { get; set; }

        [JsonIgnore]
        public double TotalCountryTests { get; set; }

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
