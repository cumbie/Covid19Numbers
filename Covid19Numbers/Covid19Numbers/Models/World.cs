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

        public DateTime UpdateTime => TimeZoneInfo.ConvertTimeFromUtc(DateTimeOffset.FromUnixTimeMilliseconds(this.Updated).DateTime, TimeZoneInfo.Local);

        [JsonProperty("cases")]
        public int Cases { get; set; }

        //[JsonProperty("todayCases")]
        //public int TodayCases { get; set; }

        [JsonProperty("deaths")]
        public int Deaths { get; set; }

        //[JsonProperty("todayDeaths")]
        //public int TodayDeaths { get; set; }

        [JsonProperty("recovered")]
        public int Recovered { get; set; }

        [JsonProperty("active")]
        public int Active { get; set; }

        [JsonProperty("critical")]
        public int Critical { get; set; }

        [JsonProperty("affectedCountries")]
        public int AffectedCountries { get; set; }
    }
}
