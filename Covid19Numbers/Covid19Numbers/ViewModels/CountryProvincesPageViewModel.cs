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
    public class CountryProvincesPageViewModel : ViewModelBase
    {
        ICovidApi _covidApi;

        public CountryProvincesPageViewModel(INavigationService navigationService, ICovidApi covidApi)
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
            set
            {
                SetProperty(ref _selectedProvince, value);
                if (this.IsActive && _selectedProvince.ProvinceName.ToLower() != "mainland")
                {
                    Settings.SelectedProvince = _selectedProvince.ProvinceName;
                    NavigationService.NavigateAsync(nameof(Views.ProvinceStatsTabbedPage));
                }
            }
        }

        private bool _isProvinceRefreshing = false;
        public bool IsProvinceRefreshing
        {
            get => _isProvinceRefreshing;
            set => SetProperty(ref _isProvinceRefreshing, value);
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

            if (!ccChanged && this.ProvinceStats != null)
                return;

            await Refresh();
        }

        private bool _abortRefresh;

        public override async Task Refresh()
        {
            if (string.IsNullOrWhiteSpace(this.CountryCode))
                return;

            // need to manage a previous refresh when country changes to avoid
            // getting provinces from other countries mixed together
            if (this.IsProvinceRefreshing)
            {
                // abort -> break existing refresh loop
                _abortRefresh = true;
                // wait until IsRefreshing == false
                while (this.IsProvinceRefreshing)
                {
                    await System.Threading.Tasks.Task.Delay(250);
                }

                // proceed
            }

            this.ProvinceStats.Clear();

            this.IsProvinceRefreshing = true;
            try
            {
                if (this.ProvinceOrState == "Provinces")
                {
                    var provinceNames = await _covidApi.GetCountryProvinces(this.CountryCode);
                    foreach (var provinceName in provinceNames)
                    {
                        if (_abortRefresh)
                        {
                            _abortRefresh = false;
                            break;
                        }

                        var stats = await _covidApi.GetProvinceStats(this.CountryCode, provinceName);
                        this.ProvinceStats.Add(stats);
                    }
                }
                else
                {
                    var states = await _covidApi.GetUsaStates();
                    foreach (var state in states)
                    {
                        if (_abortRefresh)
                        {
                            _abortRefresh = false;
                            break;
                        }

                        var stats = await _covidApi.GetUsaStateStatsAsProvince(state);
                        if (stats != null)
                            this.ProvinceStats.Add(stats);
                    }
                }
            }
            catch { }
            finally
            {
                this.IsProvinceRefreshing = false;
            }
        }
    }
}
