using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Covid19Numbers.Api;
using Covid19Numbers.Models;

namespace Covid19Numbers.ViewModels
{
    public class CountryStatsPageViewModel : ViewModelBase
    {
        ICovidApi _covidApi;

        public CountryStatsPageViewModel(INavigationService navigationService, ICovidApi covidApi)
            : base(navigationService)
        {
            _covidApi = covidApi;
        }

        #region Propeties

        private string _countryCode;
        public string CountryCode
        {
            get => _countryCode;
            set => SetProperty(ref _countryCode, value);
        }

        private Country _countryStats;
        public Country CountryStats
        {
            get => _countryStats;
            set => SetProperty(ref _countryStats, value);
        }

        #endregion

        protected override async void RaiseIsActiveChanged()
        {
            base.RaiseIsActiveChanged();

            this.CountryCode = Settings.MyCountryCode;
            Refresh();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            this.CountryCode = Settings.MyCountryCode;
            Refresh();
        }

        private async Task Refresh()
        {
            if (string.IsNullOrWhiteSpace(this.CountryCode))
                return;

            this.CountryStats = await _covidApi.GetCountryStats(this.CountryCode);
        }
    }
}
