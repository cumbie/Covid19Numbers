using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Covid19Numbers.Models;
using System.Threading.Tasks;
using Covid19Numbers.Api;

namespace Covid19Numbers.ViewModels
{
    public class SelectCountryPageViewModel : ViewModelBase
    {
        ICovidApi _covidApi;

        public SelectCountryPageViewModel(INavigationService navigationService, ICovidApi covidApi)
            : base(navigationService)
        {
            Title = "Select Country";
            _covidApi = covidApi;
        }

        #region Properties

        private ObservableCollection<SelectCountryModel> _countries = new ObservableCollection<SelectCountryModel>();
        public ObservableCollection<SelectCountryModel> Countries
        {
            get => _countries;
            set { SetProperty(ref _countries, value); }
        }

        private SelectCountryModel _selectedCountry;
        public SelectCountryModel SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                SetProperty(ref _selectedCountry, value);
                if (_selectedCountry != null)
                {
                    Settings.MyCountryCode = _selectedCountry.CountryCode;
                }
            }
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
            await Refresh();
        }

        public override async Task Refresh()
        {
            var countries = await _covidApi.GetCountryList();
            countries = countries.OrderByDescending(c => c.Cases).ToList();
            this.Countries = new ObservableCollection<SelectCountryModel>(countries);

            if (this.Countries != null)
            {
                this.SelectedCountry = this.Countries.FirstOrDefault(c => c.CountryCode == Settings.MyCountryCode);
            }
        }
    }
}
