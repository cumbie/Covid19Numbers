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

        public string CountryName { get; set; }

        public string ProvinceName { get; set; }

        public int Cases { get; set; }

        public int Deaths { get; set; }

        public int Recovered { get; set; }

        // only populated for USA
        public State StateStats { get; set; } = new State();

        #region Internal Values

        [JsonIgnore]
        public bool IsUsaState => this.CountryName.ToLower().StartsWith("us");

        [JsonIgnore]
        public double TotalCountryCases { get; set; }

        [JsonIgnore]
        public double TotalCountryDeaths { get; set; }

        [JsonIgnore]
        public double PercentCases => Math.Round(100.0 * (double)this.Cases / this.TotalCountryCases, 3);

        [JsonIgnore]
        public double PercentDeaths => (this.Cases != 0) ? Math.Round(100.0 * (double)this.Deaths / this.Cases, 3) : 0;

        [JsonIgnore]
        public double PercentTotalDeaths => Math.Round(100.0 * (double)this.Deaths / this.TotalCountryDeaths, 3);

        [JsonIgnore]
        public double PercentRecovered => (this.Cases != 0) ? Math.Round(100.0 * (double)this.Recovered / this.Cases, 3) : 0;

        #endregion
    }
}
