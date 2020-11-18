using Covid19Numbers.Api;
using Covid19Numbers.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Covid19Numbers.ViewModels
{
    public class ProvinceHistoricalPageViewModel : ViewModelBase
    {
        ICovidApi _covidApi;

        public ProvinceHistoricalPageViewModel(INavigationService navigationService, ICovidApi covidApi)
            : base(navigationService)
        {
            _covidApi = covidApi;
        }

        #region Propeties

        public static string LastCountryCode;

        private string _countryCode;
        public string CountryCode
        {
            get => _countryCode;
            set => SetProperty(ref _countryCode, value);
        }

        private string _provinceName;
        public string ProvinceName
        {
            get => _provinceName;
            set => SetProperty(ref _provinceName, value);
        }

        private ObservableCollection<DayStatistics> _history;
        public ObservableCollection<DayStatistics> History
        {
            get => _history;
            set => SetProperty(ref _history, value);
        }

        private DayStatistics _selectedStat;
        public DayStatistics SelectedStat
        {
            get => _selectedStat;
            set => SetProperty(ref _selectedStat, value);
        }

        private DateTime _casesHighDay;
        public DateTime CasesHighDay
        {
            get => _casesHighDay;
            set => SetProperty(ref _casesHighDay, value);
        }

        private DateTime _deathsHighDay;
        public DateTime DeathsHighDay
        {
            get => _deathsHighDay;
            set => SetProperty(ref _deathsHighDay, value);
        }

        private DateTime _casesLowDay;
        public DateTime CasesLowDay
        {
            get => _casesLowDay;
            set => SetProperty(ref _casesLowDay, value);
        }

        private DateTime _deathsLowDay;
        public DateTime DeathsLowDay
        {
            get => _deathsLowDay;
            set => SetProperty(ref _deathsLowDay, value);
        }

        private int _casesHigh;
        public int CasesHigh
        {
            get => _casesHigh;
            set => SetProperty(ref _casesHigh, value);
        }

        private int _deathsHigh;
        public int DeathsHigh
        {
            get => _deathsHigh;
            set => SetProperty(ref _deathsHigh, value);
        }

        private int _casesLow;
        public int CasesLow
        {
            get => _casesLow;
            set => SetProperty(ref _casesLow, value);
        }

        private int _deathsLow;
        public int DeathsLow
        {
            get => _deathsLow;
            set => SetProperty(ref _deathsLow, value);
        }

        #endregion

        protected async override void RaiseIsActiveChanged()
        {
            base.RaiseIsActiveChanged();

            if (this.IsActive)
                await HandlePageEntry();
        }

        private async Task HandlePageEntry()
        {
            bool ccChanged = (LastCountryCode != Settings.MyCountryCode);

            this.CountryCode = Settings.MyCountryCode;
            LastCountryCode = this.CountryCode;
            this.ProvinceName = Settings.SelectedProvince;

            //if (!ccChanged && this.History != null && _covidApi.ValidProvinceHistory)
            //    return;

            await Refresh();
        }

        public override async Task Refresh()
        {
            var countyHistories = await _covidApi.GetProvinceHistory(this.CountryCode, this.ProvinceName, 1000);
            // aggregate all county totals into one history
            ProvinceHistory history = GetProvinceTotals(countyHistories);
            var stats = history.GetHistoricalStats();

            this.History = new ObservableCollection<DayStatistics>(stats);

            // get highs and lows
            var cases = this.History.OrderByDescending(h => h.NewCases);
            var deaths = this.History.OrderByDescending(h => h.NewDeaths);
            var highCases = cases.First();
            var highDeaths = deaths.First();
            var lowCases = cases.Where(h => h.Date > DateTime.Now.AddDays(-30) && h.NewCases >= 0).Last();
            var lowDeaths = deaths.Where(h => h.Date > DateTime.Now.AddDays(-30) && h.NewDeaths >= 0).Last();

            this.CasesHighDay = highCases.Date;
            this.CasesHigh = highCases.NewCases;
            this.DeathsHighDay = highDeaths.Date;
            this.DeathsHigh = highDeaths.NewDeaths;

            this.CasesLowDay = lowCases.Date;
            this.CasesLow = lowCases.NewCases;
            this.DeathsLowDay = lowDeaths.Date;
            this.DeathsLow = lowDeaths.NewDeaths;
        }

        private ProvinceHistory GetProvinceTotals(List<ProvinceHistory> histories)
        {
            ProvinceHistory history = new ProvinceHistory();
            var firstHist = histories.First();
            history.CountryName = firstHist.CountryName;
            history.ProvinceName = firstHist.ProvinceName;
            history.Timeline = new CountryTimeline();

            foreach (var hist in histories)
            {
                foreach (var kvp in hist.Timeline.Cases)
                {
                    if (!history.Timeline.Cases.ContainsKey(kvp.Key))
                        history.Timeline.Cases.Add(kvp.Key, kvp.Value);
                    else
                        history.Timeline.Cases[kvp.Key] += kvp.Value;
                }
                foreach (var kvp in hist.Timeline.Deaths)
                {
                    if (!history.Timeline.Deaths.ContainsKey(kvp.Key))
                        history.Timeline.Deaths.Add(kvp.Key, kvp.Value);
                    else
                        history.Timeline.Deaths[kvp.Key] += kvp.Value;
                }
            }

            return history;
        }
    }
}
