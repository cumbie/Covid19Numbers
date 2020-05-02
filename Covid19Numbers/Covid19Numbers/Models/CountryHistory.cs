using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Covid19Numbers.Models
{
    public class CountryHistory
    {
        public CountryHistory()
        {
        }

        [JsonProperty("country")]
        public string CountryName { get; set; }

        [JsonProperty("provinces")]
        public List<string> Provinces { get; set; }

        [JsonProperty("timeline")]
        public CountryTimeline Timeline { get; set; }

        public List<DayStatistics> GetHistoricalStats()
        {
            List<DayStatistics> stats = new List<DayStatistics>();

            var cases = this.Timeline.Cases;
            var deaths = this.Timeline.Deaths;
            var recovered = this.Timeline.Recovered;

            foreach (var date in cases.Keys)
            {
                var yesterday = date.AddDays(-1);

                var newCasesToday = cases.ContainsKey(yesterday) ? (cases[date] - cases[yesterday]) : 0;
                var newDeathsToday = deaths.ContainsKey(yesterday) ? (deaths[date] - deaths[yesterday]) : 0;
                var newRecoveredToday = recovered.ContainsKey(yesterday) ? (recovered[date] - recovered[yesterday]) : 0;

                var lastStat = stats.LastOrDefault();

                var stat = new DayStatistics
                {
                    ContextName = this.CountryName,

                    Date = date,
                    Cases = cases[date],
                    Deaths = deaths[date],
                    Recovered = recovered[date],

                    NewCases = newCasesToday,
                    NewDeaths = newDeathsToday,
                    NewRecovered = newRecoveredToday,

                    CasesUp = (cases.ContainsKey(yesterday) && lastStat != null) && newCasesToday > lastStat.NewCases,
                    DeathsUp = (deaths.ContainsKey(yesterday) && lastStat != null) && newDeathsToday > lastStat.NewDeaths,
                    RecoveredUp = (recovered.ContainsKey(yesterday) && lastStat != null) && newRecoveredToday > lastStat.NewRecovered
                };

                stat.DeltaCases = (lastStat != null) ? (stat.NewCases - lastStat.NewCases) : stat.NewCases;
                stat.DeltaDeaths = (lastStat != null) ? (stat.NewDeaths - lastStat.NewDeaths) : stat.NewDeaths;
                stat.DeltaRecovered = (lastStat != null) ? (stat.NewRecovered - lastStat.NewRecovered) : stat.NewRecovered;

                stats.Add(stat);
            }

            return stats.OrderByDescending(h => h.Date).ToList();
        }
    }
}
