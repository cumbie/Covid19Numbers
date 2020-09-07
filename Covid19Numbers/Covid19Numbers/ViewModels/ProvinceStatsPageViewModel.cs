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
    public class ProvinceStatsPageViewModel : ViewModelBase
    {
        ICovidApi _covidApi;

        public ProvinceStatsPageViewModel(INavigationService navigationService, ICovidApi covidApi)
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

        private string _provinceName;
        public string ProvinceName
        {
            get => _provinceName;
            set => SetProperty(ref _provinceName, value);
        }

        private Province _provinceStats;
        public Province ProvinceStats
        {
            get => _provinceStats;
            set => SetProperty(ref _provinceStats, value);
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
            this.CountryCode = Settings.MyCountryCode;
            this.ProvinceName = Settings.SelectedProvince;
            
            //if (!ccChanged && this.CountryStats != null && _covidApi.ValidCountryStats)
            //    return;

            await Refresh();
        }

        public override async Task Refresh()
        {
            if (string.IsNullOrWhiteSpace(this.CountryCode))
                return;

            this.ProvinceStats = await _covidApi.GetProvinceStats(this.CountryCode, this.ProvinceName);
        }
    }
}
