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

        #endregion

        protected override async void RaiseIsActiveChanged()
        {
            base.RaiseIsActiveChanged();

            if (this.GlobalStats != null && _covidApi.ValidGlobalStats)
                return;

            await Refresh();
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (this.GlobalStats != null && _covidApi.ValidGlobalStats)
                return;

            await Refresh();
        }

        public override async Task Refresh()
        {
            this.GlobalStats = await _covidApi.GetGlobalStats();
        }
    }
}
