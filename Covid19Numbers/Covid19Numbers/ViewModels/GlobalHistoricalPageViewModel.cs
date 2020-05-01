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
    public class GlobalHistoricalPageViewModel : ViewModelBase
    {
        ICovidApi _covidApi;

        public GlobalHistoricalPageViewModel(INavigationService navigationService, ICovidApi covidApi)
            : base(navigationService)
        {
            _covidApi = covidApi;
        }

        #region Propeties

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

            if (this.History != null && _covidApi.ValidGlobalHistory)
                return;

            await Refresh();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (this.History != null && _covidApi.ValidGlobalHistory)
                return;

            await Refresh();
        }

        public override async Task Refresh()
        {
            var history = await _covidApi.GetGlobalHistory(1000);
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
    }
}
