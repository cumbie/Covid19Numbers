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
        int _counterDelay = 1000;

        HttpClient _client;

        string _country = "US";

        string _worldUrl = "https://corona.lmao.ninja/all";
        string _countryUrl = "https://corona.lmao.ninja/countries/";
        string _statesUrl = "https://corona.lmao.ninja/states";

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "COVID-19 Numbers";

            _running = false;

            _client = new HttpClient();

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

        private string _myCountry;
        public string MyCountry
        {
            get { return _myCountry; }
            set { SetProperty(ref _myCountry, value); }
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

        #endregion

        int _stateCnt = 0;
        void CounterThread()
        {
            Thread.Sleep(_counterStartDelay);

            _running = true;
            try
            {
                while (_running)
                {
                    if (this.MyCountry == null)
                    {
                        SetMyCountry();
                    }
                    
                    var worldResponse = _client.GetAsync(_worldUrl).GetAwaiter().GetResult();
                    //var worldson = "{\"cases\":597458,\"deaths\":27370,\"recovered\":133373,\"updated\":1585375470343,\"active\":436715}";
                    var worldson = worldResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var world = JsonConvert.DeserializeObject<World>(worldson);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.TotalCases = world.Cases;
                        this.TotalDeaths = world.Deaths;
                        this.WorldLastUpdate = world.UpdateTime;
                    });

                    var countryResponse = _client.GetAsync($"{_countryUrl}{this.MyCountry}").GetAwaiter().GetResult();
                    var countryJson = countryResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    //var countryJson = "{'country':'USA','countryInfo':{'_id':840,'lat':38,'long':-97,'flag':'https://raw.githubusercontent.com/NovelCOVID/API/master/assets/flags/us.png','iso3':'USA','iso2':'US'},'cases':112815,'todayCases':8689,'deaths':1880,'todayDeaths':184,'recovered':3219,'active':107716,'critical':2666,'casesPerOneMillion':341,'deathsPerOneMillion':6}";
                    var country = JsonConvert.DeserializeObject<Country>(countryJson);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.MyCountry = country.CountryName;
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

                    Thread.Sleep(_counterDelay);
                }
            }
            catch (Exception x)
            {
                Console.WriteLine($"CounterError: {x.ToString()}");
            }
        }

        private void SetMyCountry()
        {
            this.MyCountry = "USA";
            // TODO: use GPS to get country by geolocation
        }

    }
}
