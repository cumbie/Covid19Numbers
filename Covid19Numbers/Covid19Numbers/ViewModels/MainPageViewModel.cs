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
        int _counterStartDelay = 3000;
        int _counterDelay = 1000;

        HttpClient _client;

        string _country = "US";

        string _countryUrl = "https://corona.lmao.ninja/countries/";
        string _statesUrl = "https://corona.lmao.ninja/states";

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Main Page";

            _running = false;

            _client = new HttpClient();

            Task.Factory.StartNew(CounterThread);
        }

        private int _totalCases = 0;
        public int TotalCases
        {
            get { return _totalCases; }
            set { SetProperty(ref _totalCases, value); }
        }

        private int _todayCases = 0;
        public int TodayCases
        {
            get { return _todayCases; }
            set { SetProperty(ref _todayCases, value); }
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
                    var countryResponse = _client.GetAsync($"{_countryUrl}{_country}").GetAwaiter().GetResult();
                    var countryJson = countryResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var country = ParseCountry(countryJson);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.TotalCases = country.Cases;
                        this.TodayCases = country.TodayCases;
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

        private Country ParseCountry(string json)
        {
            return JsonConvert.DeserializeObject<Country>(json);
        }
    }
}
