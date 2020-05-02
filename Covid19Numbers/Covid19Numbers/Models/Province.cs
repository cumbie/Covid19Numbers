using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Covid19Numbers.Models
{
    public class Province
    {
        public Province()
        {
        }

        [JsonProperty("country")]
        public string CountryName { get; set; }

        [JsonProperty("province")]
        public string ProvinceName { get; set; }

        [JsonProperty("timeline")]
        public CountryTimeline Timeline { get; set; }

        #region Internal Values

        [JsonIgnore]
        public int Cases => Timeline?.TotalCases ?? 0;

        [JsonIgnore]
        public int Deaths => Timeline?.TotalDeaths ?? 0;

        [JsonIgnore]
        public int Recovered => Timeline?.TotalRecovered ?? 0;

        [JsonIgnore]
        public double TotalGlobalCases { get; set; }

        [JsonIgnore]
        public double TotalGlobalDeaths { get; set; }

        [JsonIgnore]
        public double PercentCases => Math.Round(100.0 * (double)this.Cases / this.TotalGlobalCases, 3);

        [JsonIgnore]
        public double PercentDeaths => Math.Round(100.0 * (double)this.Deaths / this.TotalGlobalDeaths, 3);

        [JsonIgnore]
        public double PercentRecovered => (this.Cases != 0) ? Math.Round(100.0 * (double)this.Recovered / this.Cases, 3) : 0;

        #endregion
    }
}
