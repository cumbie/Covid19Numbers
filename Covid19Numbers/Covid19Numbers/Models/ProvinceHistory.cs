using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Covid19Numbers.Models
{
    public class ProvinceHistory
    {
        public ProvinceHistory()
        {
        }

        [JsonProperty("country")]
        public string CountryName { get; set; }

        [JsonProperty("province")]
        public string ProvinceName { get; set; }

        [JsonProperty("timeline")]
        public CountryTimeline Timeline { get; set; }

        [JsonProperty("county")]
        public string County { get; set; }

        // TODO: check if we need these for history: ....
        #region Internal Values

        [JsonIgnore]
        public int Cases => Timeline?.TotalCases ?? 0;

        [JsonIgnore]
        public int Deaths => Timeline?.TotalDeaths ?? 0;

        [JsonIgnore]
        public int Recovered => Timeline?.TotalRecovered ?? 0;

        [JsonIgnore]
        public double TotalCountryCases { get; set; }

        [JsonIgnore]
        public double TotalCountryDeaths { get; set; }

        [JsonIgnore]
        public double PercentCases => Math.Round(100.0 * (double)this.Cases / this.TotalCountryCases, 3);

        [JsonIgnore]
        public double PercentDeaths => Math.Round(100.0 * (double)this.Deaths / this.TotalCountryDeaths, 3);

        [JsonIgnore]
        public double PercentRecovered => (this.Cases != 0) ? Math.Round(100.0 * (double)this.Recovered / this.Cases, 3) : 0;

        #endregion
    }
}
