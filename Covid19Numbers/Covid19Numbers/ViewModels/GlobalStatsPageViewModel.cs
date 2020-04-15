using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Covid19Numbers.Api;
using Covid19Numbers.Models;

namespace Covid19Numbers.ViewModels
{
    public class GlobalStatsPageViewModel : ViewModelBase
    {
        ICovidApi _covidApi;

        public GlobalStatsPageViewModel(INavigationService navigationService, ICovidApi covidApi)
            : base(navigationService)
        {
            _covidApi = covidApi;
        }

        #region Propeties

        private World _globalStats;
        public World GlobalStats
        {
            get => _globalStats;
            set => SetProperty(ref _globalStats, value);
        }

        private ObservableCollection<WorldDayStat> _history;
        public ObservableCollection<WorldDayStat> History
        {
            get => _history;
            set => SetProperty(ref _history, value);
        }

        private ObservableCollection<string> _history2 = new ObservableCollection<string>();
        public ObservableCollection<string> History2
        {
            get => _history2;
            set => SetProperty(ref _history2, value);
        }

        private WorldDayStat _selectedStat;
        public WorldDayStat SelectedStat
        {
            get => _selectedStat;
            set => SetProperty(ref _selectedStat, value);
        }
        //private string _selectedStat;
        //public string SelectedStat
        //{
        //    get => _selectedStat;
        //    set => SetProperty(ref _selectedStat, value);
        //}

        #endregion

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            //List<string> hhh = new List<string>();
            //hhh.AddRange(new List<string>{"ajfkdsjf", "afdsfds", "afdsaf"});

            //this.History2 = new ObservableCollection<string>(hhh);
            Refresh();
        }

        private async Task Refresh()
        {
            this.GlobalStats = await _covidApi.GetGlobalStats();

            var worldHistory = await _covidApi.GetGlobalHistory();
            var stats = worldHistory.GetHistoricalStats();
            
            this.History = new ObservableCollection<WorldDayStat>(stats);
        }
    }
}
