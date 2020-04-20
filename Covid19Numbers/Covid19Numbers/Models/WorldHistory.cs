using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Covid19Numbers.Models
{
    public class WorldHistory
    {
        [JsonProperty("cases")]
        public IDictionary<DateTime, int> Cases { get; set; }

        [JsonProperty("deaths")]
        public IDictionary<DateTime, int> Deaths { get; set; }

        [JsonProperty("recovered")]
        public IDictionary<DateTime, int> Recovered { get; set; }

        public List<WorldDayStat> GetHistoricalStats()
        {
            List<WorldDayStat> stats = new List<WorldDayStat>();

            // Cases, Deaths, and Recovered should be the same
            //int numDays = this.Cases.Count;
            //if (!(this.Cases.Count == this.Deaths.Count &&
            //    this.Deaths.Count == this.Recovered.Count))
            //    numDays = Math.Max(Math.Max(this.Cases.Count, this.Deaths.Count), this.Recovered.Count);
            // .. if we have different sizes, then just make all the same, filling empty with zeros
            // TODO

            foreach (var date in this.Cases.Keys)
            {
                var yesterday = date.AddDays(-1);

                var newCasesToday = this.Cases.ContainsKey(yesterday) ? (this.Cases[date] - this.Cases[yesterday]) : 0;
                var newDeathsToday = this.Deaths.ContainsKey(yesterday) ? (this.Deaths[date] - this.Deaths[yesterday]) : 0;
                var newRecoveredToday = this.Recovered.ContainsKey(yesterday) ? (this.Recovered[date] - this.Recovered[yesterday]) : 0;

                var lastStat = stats.LastOrDefault();

                var stat = new WorldDayStat
                {
                    Date = date,
                    Cases = this.Cases[date],
                    Deaths = this.Deaths[date],
                    Recovered = this.Recovered[date],

                    NewCases = newCasesToday,
                    NewDeaths = newDeathsToday,
                    NewRecovered = newRecoveredToday,

                    CasesUp = (this.Cases.ContainsKey(yesterday) && lastStat != null) &&  newCasesToday > lastStat.NewCases,
                    DeathsUp = (this.Deaths.ContainsKey(yesterday) && lastStat != null) && newDeathsToday > lastStat.NewDeaths,
                    RecoveredUp = (this.Recovered.ContainsKey(yesterday) && lastStat != null) && newRecoveredToday > lastStat.NewRecovered
                };

                stat.DeltaCases = (lastStat != null) ? (stat.NewCases - lastStat.NewCases) : stat.NewCases;
                stat.DeltaDeaths = (lastStat != null) ? (stat.NewDeaths - lastStat.NewDeaths) : stat.NewDeaths;
                stat.DeltaRecovered = (lastStat != null) ? (stat.NewRecovered - lastStat.NewRecovered) : stat.NewRecovered;

                stats.Add(stat);
            }

            return stats.OrderByDescending(h => h.Date).ToList();
        }
    }

    public class WorldDayStat
    {
        public DateTime Date { get; set; }
        public int Cases { get; set; }
        public int Deaths { get; set; }
        public int Recovered { get; set; }

        public int NewCases { get; set; }
        public int NewDeaths { get; set; }
        public int NewRecovered { get; set; }

        public int DeltaCases { get; set; }
        public int DeltaDeaths { get; set; }
        public int DeltaRecovered { get; set; }

        public bool CasesUp { get; set; }
        public bool DeathsUp { get; set; }
        public bool RecoveredUp { get; set; }
    }
}
