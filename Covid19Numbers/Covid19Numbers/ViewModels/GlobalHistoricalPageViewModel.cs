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

        private ObservableCollection<WorldDayStat> _history;
        public ObservableCollection<WorldDayStat> History
        {
            get => _history;
            set => SetProperty(ref _history, value);
        }

        private WorldDayStat _selectedStat;
        public WorldDayStat SelectedStat
        {
            get => _selectedStat;
            set => SetProperty(ref _selectedStat, value);
        }

        #endregion

        protected async override void RaiseIsActiveChanged()
        {
            base.RaiseIsActiveChanged();

            if (this.History != null && _covidApi.GlobalHistoryLastUpdate.AddMilliseconds(Constants.RefreshMaxMs) > DateTime.Now)
                return;

            await Refresh();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (this.History != null && _covidApi.GlobalHistoryLastUpdate.AddMilliseconds(Constants.RefreshMaxMs) > DateTime.Now)
                return;

            await Refresh();
        }

        public override async Task Refresh()
        {
            var worldHistory = await _covidApi.GetGlobalHistory(1000);
            var stats = worldHistory.GetHistoricalStats();

            this.History = new ObservableCollection<WorldDayStat>(stats);
        }
    }
}
