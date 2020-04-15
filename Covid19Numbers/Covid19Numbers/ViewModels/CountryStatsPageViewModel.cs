using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Covid19Numbers.Api;

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

        #endregion

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            this.CountryCode = (string)parameters["CountryCode"];
            Refresh();
        }

        private async Task Refresh()
        {
            if (string.IsNullOrWhiteSpace(this.CountryCode))
                return;

            var stats = await _covidApi.GetCountryStats(this.CountryCode);
        }
    }
}
