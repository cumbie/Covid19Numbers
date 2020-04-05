using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Covid19Numbers.Models;

namespace Covid19Numbers.ViewModels
{
    public class SelectCountryPageViewModel : ViewModelBase
    {
        public SelectCountryPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Select Country";
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

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            this.Countries = new ObservableCollection<SelectCountryModel>(Settings.AllCountries);

            if (this.Countries != null)
            {
                this.SelectedCountry = this.Countries.FirstOrDefault(c => c.CountryCode == Settings.MyCountryCode);
            }
        }
    }
}
