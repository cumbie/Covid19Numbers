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
using Covid19Numbers.Models;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Covid19Numbers.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        bool _running = false;
        int _counterStartDelay = 1000;
        int _counterDelay = 1000 * 60 * 2; // 2mins

        HttpClient _client;

        string _country = "US";

        string _worldUrl = "https://corona.lmao.ninja/all";
        string _countryUrl = "https://corona.lmao.ninja/countries/";
        string _statesUrl = "https://corona.lmao.ninja/states";

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = Constants.AppName;

            _running = false;
            _client = new HttpClient();

            this.AdUnitID = (Device.RuntimePlatform == Device.iOS)
                ? Constants.AdMobAdUnitID_ad01_iOS
                : Constants.AdMobAdUnitID_ad01_Android;

            RefreshCountriesList();

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
            get { return _totalCases; }
            set { SetProperty(ref _totalCases, value); }
        }

        private int _totalDeaths = 0;
        public int TotalDeaths
        {
            get { return _totalDeaths; }
            set { SetProperty(ref _totalDeaths, value); }
        }

        private DateTime _worldLastUpdate;
        public DateTime WorldLastUpdate
        {
            get { return _worldLastUpdate; }
            set { SetProperty(ref _worldLastUpdate, value); }
        }

        private string _myCountryCode;
        public string MyCountryCode
        {
            get { return _myCountryCode; }
            set { SetProperty(ref _myCountryCode, value); }
        }

        private string _flagImageUrl;
        public string FlagImageUrl
        {
            get { return _flagImageUrl; }
            set
            {
                if (value != _flagImageUrl)
                    SetProperty(ref _flagImageUrl, value);
            }
        }

        private int _totalCountryCases = 0;
        public int TotalCountryCases
        {
            get { return _totalCountryCases; }
            set { SetProperty(ref _totalCountryCases, value); }
        }

        private int _totalCountryDeaths = 0;
        public int TotalCountryDeaths
        {
            get { return _totalCountryDeaths; }
            set { SetProperty(ref _totalCountryDeaths, value); }
        }

        private int _totalCountryTodayCases = 0;
        public int TotalCountryTodayCases
        {
            get { return _totalCountryTodayCases; }
            set { SetProperty(ref _totalCountryTodayCases, value); }
        }

        private int _totalCountryTodayDeaths = 0;
        public int TotalCountryTodayDeaths
        {
            get { return _totalCountryTodayDeaths; }
            set { SetProperty(ref _totalCountryTodayDeaths, value); }
        }

        private string _adUnitId;
        public string AdUnitID
        {
            get => _adUnitId;
            set => SetProperty(ref _adUnitId, value);
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
                var resp = await _client.GetAsync($"{_countryUrl}");
                var json = await resp.Content.ReadAsStringAsync();
                Settings.AllCountries = JsonConvert.DeserializeObject<List<SelectCountryModel>>(json);
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
                while (_running)
                {
                    RefreshStats().GetAwaiter().GetResult();

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
            // TODO: move all API calls to a GetStatsService class

            var worldResponse = await _client.GetAsync(_worldUrl);
            //var worldson = "{\"cases\":597458,\"deaths\":27370,\"recovered\":133373,\"updated\":1585375470343,\"active\":436715}";
            var worldson = await worldResponse.Content.ReadAsStringAsync();
            var world = JsonConvert.DeserializeObject<World>(worldson);

            Device.BeginInvokeOnMainThread(() =>
            {
                this.TotalCases = world.Cases;
                this.TotalDeaths = world.Deaths;
                this.WorldLastUpdate = world.UpdateTime;
            });
            
            var countryResponse = await _client.GetAsync($"{_countryUrl}{this.MyCountryCode}");
            var countryJson = await countryResponse.Content.ReadAsStringAsync();
            //var countryJson = "{'country':'USA','countryInfo':{'_id':840,'lat':38,'long':-97,'flag':'https://raw.githubusercontent.com/NovelCOVID/API/master/assets/flags/us.png','iso3':'USA','iso2':'US'},'cases':112815,'todayCases':8689,'deaths':1880,'todayDeaths':184,'recovered':3219,'active':107716,'critical':2666,'casesPerOneMillion':341,'deathsPerOneMillion':6}";
            var country = JsonConvert.DeserializeObject<Country>(countryJson);

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
            await RefreshCountriesList();

            await RefreshStats();
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
