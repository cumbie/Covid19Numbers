using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Covid19Numbers.Api;
using Covid19Numbers.Models;
using System.Collections.ObjectModel;

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

        public static string LastCountryCode;

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

            if (this.IsActive)
                await HandlePageEntry();
        }

        private async Task HandlePageEntry()
        {
            bool ccChanged = (LastCountryCode != Settings.MyCountryCode);

            this.CountryCode = Settings.MyCountryCode;
            LastCountryCode = this.CountryCode;

            if (!ccChanged && this.CountryStats != null && _covidApi.ValidCountryStats)
                return;

            await Refresh();
        }

        public override async Task Refresh()
        {
            if (string.IsNullOrWhiteSpace(this.CountryCode))
                return;

            this.CountryStats = await _covidApi.GetCountryStats(this.CountryCode);
        }
    }
}
