using System;
namespace Covid19Numbers.Models
{
    public class DayStatistics
    {
        public string ContextName { get; set; }

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
