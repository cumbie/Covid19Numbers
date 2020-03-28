using System;
using Newtonsoft.Json;

namespace Covid19Numbers.Models
{
    public class World
    {
        public World()
        {
        }

        [JsonProperty("cases")]
        public int Cases { get; set; }

        [JsonProperty("deaths")]
        public int Deaths { get; set; }

        [JsonProperty("recovered")]
        public int Recovered { get; set; }

        [JsonProperty("updated")]
        public long Updated { get; set; }

        [JsonProperty("active")]
        public int Active { get; set; }

        public DateTime UpdateTime => DateTimeOffset.FromUnixTimeMilliseconds(this.Updated).DateTime;
    }
}
