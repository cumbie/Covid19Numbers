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
            set
            {
                SetProperty(ref _countryCode, value);
                this.ProvinceOrState = (_countryCode.ToLower() == "us" || _countryCode.ToLower() == "usa") ? "States" : "Provinces";
            }
        }

        private Country _countryStats;
        public Country CountryStats
        {
            get => _countryStats;
            set => SetProperty(ref _countryStats, value);
        }

        private string _provinceOrState;
        public string ProvinceOrState
        {
            get => _provinceOrState;
            set => SetProperty(ref _provinceOrState, value);
        }

        private ObservableCollection<Province> _provinceStats = new ObservableCollection<Province>();
        public ObservableCollection<Province> ProvinceStats
        {
            get => _provinceStats;
            set => SetProperty(ref _provinceStats, value);
        }

        private Province _selectedProvince;
        public Province SelectedProvince
        {
            get => _selectedProvince;
            set => SetProperty(ref _selectedProvince, value);
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

            this.ProvinceStats.Clear();

            List<Province> provinceStats = new List<Province>();
            if (this.ProvinceOrState == "Provinces")
            {
                var provinceNames = await _covidApi.GetCountryProvinces(this.CountryCode);
                foreach (var provinceName in provinceNames)
                {
                    var stats = await _covidApi.GetProvinceStats(this.CountryCode, provinceName, 1000);
                    this.ProvinceStats.Add(stats);
                }
            }
            else
            {
                // var states = await _covidApi.GetUsaStates();
                foreach (var state in states)
                {
                    var stats = await _covidApi.GetUsaStateStatsAsProvince(state, 1000);
					this.ProvinceStats.Add(stats);
				}
            }
        }
    }
}
