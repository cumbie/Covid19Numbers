using System;
using Newtonsoft.Json;

namespace Covid19Numbers.Models
{
    public class World
    {
        public World()
        {
        }

        //{
        //  "updated": 1586500201754,
        //  "cases": 1605372,
        //  "todayCases": 1720,
        //  "deaths": 95753,
        //  "todayDeaths": 61,
        //  "recovered": 356952,
        //  "active": 1152667,
        //  "critical": 49143,
        //  "casesPerOneMillion": 206,
        //  "deathsPerOneMillion": 12,
        //  "tests": 12662352,
        //  "testsPerOneMillion": 1624.8,
        //  "affectedCountries": 212
        //}

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
