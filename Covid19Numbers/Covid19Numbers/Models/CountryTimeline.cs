using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Covid19Numbers.Models
{
    public class CountryTimeline
    {
        public CountryTimeline()
        {
            this.Cases = new Dictionary<DateTime, int>();
            this.Deaths = new Dictionary<DateTime, int>();
            this.Recovered = new Dictionary<DateTime, int>();
        }

        [JsonProperty("cases")]
        public IDictionary<DateTime, int> Cases { get; set; }

        [JsonProperty("deaths")]
        public IDictionary<DateTime, int> Deaths { get; set; }

        [JsonProperty("recovered")]
        public IDictionary<DateTime, int> Recovered { get; set; }

        #region Internal Values

        [JsonIgnore]
        public int TotalCases => GetTotalCases();

        [JsonIgnore]
        public int TotalDeaths => GetTotalDeaths();

        [JsonIgnore]
        public int TotalRecovered => GetTotalRecovered();

        #endregion

        private int GetTotalCases()
        {
            int count = 0;
            if (this.Cases != null)
            {
                count = this.Cases.Sum(key => key.Value);
            }

            return count;
        }

        private int GetTotalDeaths()
        {
            int count = 0;
            if (this.Deaths != null)
            {
                foreach (var day in this.Deaths.OrderBy(key => key.Key))
                {
                    count += day.Value - count;
                }
            }

            return count;
        }

        private int GetTotalRecovered()
        {
            int count = 0;
            if (this.Recovered != null)
            {
                foreach (var day in this.Recovered.OrderBy(key => key.Key))
                {
                    count += day.Value - count;
                }
            }

            return count;
        }
    }
}
