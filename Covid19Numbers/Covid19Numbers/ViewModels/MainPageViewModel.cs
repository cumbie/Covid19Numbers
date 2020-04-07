using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using Covid19Numbers.Api;
using Covid19Numbers.Models;

namespace Covid19Numbers.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        bool _running = false;
        int _counterStartDelay = 1000;
        int _counterDelay = 1000 * 60 * 2; // 2mins

        string _country = "US";

        ICovidApi _covidApi;

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = Constants.AppName;

            _running = false;
            _covidApi = new LmaoNinjaCovidApi();

            this.AdUnitID = (Device.RuntimePlatform == Device.iOS)
                ? Constants.AdMobAdUnitID_ad01_iOS
                : Constants.AdMobAdUnitID_ad01_Android;

            RefreshCountriesList();

            RefreshAllCommand = new DelegateCommand(RefreshAll);
            SelectCountryCommand = new DelegateCommand(SelectCountry);
            GotoSettingsCommand = new DelegateCommand(GotoSettings);
            GotoAboutCommand = new DelegateCommand(GotoAbout);

            this.MyCountryCode = Settings.MyCountryCode;

            Task.Factory.StartNew(CounterThread);
        }

        #region Properties

        private int _totalCases = 0;
        public int TotalCases
        {
            get => _totalCases;
            set => SetProperty(ref _totalCases, value);
        }

        private int _totalDeaths = 0;
        public int TotalDeaths
        {
            get => _totalDeaths;
            set => SetProperty(ref _totalDeaths, value);
        }

        private DateTime _worldLastUpdate;
        public DateTime WorldLastUpdate
        {
            get => _worldLastUpdate;
            set => SetProperty(ref _worldLastUpdate, value);
        }

        private string _myCountryCode;
        public string MyCountryCode
        {
            get => _myCountryCode;
            set => SetProperty(ref _myCountryCode, value);
        }

        private string _flagImageUrl;
        public string FlagImageUrl
        {
            get => _flagImageUrl;
            set
            {
                if (value != _flagImageUrl)
                    SetProperty(ref _flagImageUrl, value);
            }
        }

        private int _totalCountryCases = 0;
        public int TotalCountryCases
        {
            get => _totalCountryCases;
            set => SetProperty(ref _totalCountryCases, value);
        }

        private int _totalCountryDeaths = 0;
        public int TotalCountryDeaths
        {
            get => _totalCountryDeaths;
            set => SetProperty(ref _totalCountryDeaths, value);
        }

        private int _totalCountryTodayCases = 0;
        public int TotalCountryTodayCases
        {
            get => _totalCountryTodayCases;
            set => SetProperty(ref _totalCountryTodayCases, value);
        }

        private int _totalCountryTodayDeaths = 0;
        public int TotalCountryTodayDeaths
        {
            get => _totalCountryTodayDeaths;
            set => SetProperty(ref _totalCountryTodayDeaths, value);
        }

        private string _adUnitId;
        public string AdUnitID
        {
            get => _adUnitId;
            set => SetProperty(ref _adUnitId, value);
        }

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        #endregion

        #region Commands

        public DelegateCommand RefreshAllCommand { get; private set; }
        public DelegateCommand SelectCountryCommand { get; private set; }
        public DelegateCommand GotoSettingsCommand { get; private set; }
        public DelegateCommand GotoAboutCommand { get; private set; }

        #endregion

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            this.MyCountryCode = Settings.MyCountryCode;

            await RefreshStats();
        }

        private async Task RefreshCountriesList()
        {
            // TODO: this should be called when explicit refreshing and OnResume

            try
            {
                Settings.AllCountries = await _covidApi.GetCountryList();
            }
            catch (Exception x)
            {
                // log, display error, etc
            }
        }

        int _stateCnt = 0;
        void CounterThread()
        {
            Thread.Sleep(_counterStartDelay);

            _running = true;
            try
            {
                int errorCount = 0;
                while (_running)
                {                    
                    try
                    {
                        RefreshStats().GetAwaiter().GetResult();
                        errorCount = 0;
                    }
                    catch (Exception x)
                    {
                        // log
                        // if too many errors, abort, or wait a while?
                        if (errorCount++ == 5)
                        {
                            // error message
                            // abort, retry, wait for use refresh to restart
                            // if (no internet?)
                        }
                    }

                    Thread.Sleep(_counterDelay);
                }
            }
            catch (Exception x)
            {
                Console.WriteLine($"CounterError: {x.ToString()}");
            }
        }

        private async Task RefreshStats()
        {
            var world = await _covidApi.GetGlobalStats();
            Device.BeginInvokeOnMainThread(() =>
            {
                this.TotalCases = world.Cases;
                this.TotalDeaths = world.Deaths;
                this.WorldLastUpdate = world.UpdateTime;
            });

            var country = await _covidApi.GetCountryStats(this.MyCountryCode);
            Device.BeginInvokeOnMainThread(() =>
            {
                this.MyCountryCode = country.CountryName;
                this.FlagImageUrl = country.Info.FlagImageUrl;
                this.TotalCountryCases = country.Cases;
                this.TotalCountryDeaths = country.Deaths;
                this.TotalCountryTodayCases = country.TodayCases;
                this.TotalCountryTodayDeaths = country.TodayDeaths;
            });

            //if (_stateCnt++ == 5)
            //{
            //    var stateResponse = _client.GetAsync(_statesUrl).GetAwaiter().GetResult();
            //    var stateJson = stateResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            //}
        }

        public async void RefreshAll()
        {
            this.IsRefreshing = true;

            await RefreshCountriesList();

            await RefreshStats();

            this.IsRefreshing = false;
        }

        public async void SelectCountry()
        {
            await NavigationService.NavigateAsync(nameof(Views.SelectCountryPage));
        }

        public void GotoSettings()
        {

        }

        public void GotoAbout()
        {

        }
    }
}
