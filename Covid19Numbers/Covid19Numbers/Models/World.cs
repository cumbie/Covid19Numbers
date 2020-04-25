using System;
using Newtonsoft.Json;

namespace Covid19Numbers.Models
{
    public class World
    {
        public World()
        {
        }

        [JsonProperty("updated")]
        public long Updated { get; set; }

        [JsonProperty("cases")]
        public int Cases { get; set; }

        [JsonProperty("todayCases")]
        public int TodayCases { get; set; }

        [JsonProperty("deaths")]
        public int Deaths { get; set; }

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

        [JsonProperty("affectedCountries")]
        public int AffectedCountries { get; set; }

        #region Internal Values

        [JsonIgnore]
        public DateTime UpdateTime => DateTimeOffset.FromUnixTimeMilliseconds(this.Updated).DateTime;

        [JsonIgnore]
        public DateTime UpdateLocalTime => TimeZoneInfo.ConvertTimeFromUtc(UpdateTime, TimeZoneInfo.Local);

        [JsonIgnore]
        public double PercentDeaths => Math.Round(100.0 * (double)this.Deaths / this.Cases, 3);

        [JsonIgnore]
        public double PercentTodayDeaths => Math.Round(100.0 * (double)this.TodayDeaths / this.TodayCases, 3);

        [JsonIgnore]
        public double PercentRecovered => Math.Round(100.0 * (double)this.Recovered / this.Cases, 3);

        [JsonIgnore]
        public double PercentActive => Math.Round(100.0 * (double)this.Active / this.Cases, 3);

        [JsonIgnore]
        public double PercentCritical => Math.Round(100.0 * (double)this.Critical / this.Cases, 3);

        #endregion
    }
}
