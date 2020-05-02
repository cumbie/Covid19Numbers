using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Covid19Numbers.Models;
using Newtonsoft.Json;

namespace Covid19Numbers.Api
{
    public class LmaoNinjaCovidApi : ICovidApi
    {
        HttpClient _client;

        //string _baseUrl = "https://corona.lmao.ninja";
        string _baseUrl = "https://disease.sh";

        // world
        string _worldEndpoint = "/v2/all";
        string _continentsEndpoint = "/v2/continents";
        string _worldHistorialEndpoint = "/v2/historical/all";

        // country
        string _countryEndpoint = "/v2/countries/";
        string _countryHistoricalEndpoint = "/v2/historical/"; // /{country}/{province}

        // states/provinces
        string _statesEndpoint = "/states";

        private int _latestGlobalTotalCases = -1;
        private int _latestGlobalTotalDeaths = -1;
        private int _latestGlobalTotalTests = -1;

        public LmaoNinjaCovidApi()
        {
            _client = new HttpClient();
        }

        #region Properties

        public DateTime GlobalStatsLastUpdate { get; private set; }
        public DateTime GlobalHistoryLastUpdate { get; private set; }

        public DateTime CountryStatsLastUpdate { get; private set; }
        public DateTime CountryHistoryLastUpdate { get; private set; }

        public DateTime ProvinceStatsLastUpdate { get; private set; }

        public bool ValidGlobalStats => IsStatValid(GlobalStatsLastUpdate);
        public bool ValidGlobalHistory => IsStatValid(GlobalHistoryLastUpdate);

        public bool ValidCountryStats => IsStatValid(CountryStatsLastUpdate);
        public bool ValidCountryHistory => IsStatValid(CountryHistoryLastUpdate);

        public bool ValidProvinceStats => IsStatValid(ProvinceStatsLastUpdate);

        #endregion

        #region Helper Functions

        private bool IsStatValid(DateTime lastUpdate)
        {
            return lastUpdate.AddMilliseconds(Constants.RefreshMaxMs) > DateTime.Now;
        }

        private string Url(string endpoint) => $"{_baseUrl}{endpoint}";

        #endregion

        public async Task<WorldHistory> GetGlobalHistory(int days = 30) // TODO: pulldown to select days
        {
            var url = Url(_worldHistorialEndpoint);
            if (days > 0 && days != 30)
            {
                url += $"?lastdays={days}";
            }
            var response = await _client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            var history = JsonConvert.DeserializeObject<WorldHistory>(json);
            this.GlobalHistoryLastUpdate = DateTime.Now;

            return history;
        }

        public async Task<World> GetGlobalStats()
        {
            var response = await _client.GetAsync(Url(_worldEndpoint));
            var json = await response.Content.ReadAsStringAsync();

            var world = JsonConvert.DeserializeObject<World>(json);
            this.GlobalStatsLastUpdate = DateTime.Now;

            _latestGlobalTotalCases = world.Cases;
            _latestGlobalTotalDeaths = world.Deaths;
            _latestGlobalTotalTests = world.Tests;

            return world;
        }

        public async Task<List<SelectCountryModel>> GetCountryList()
        {
            var response = await _client.GetAsync($"{Url(_countryEndpoint)}");
            var json = await response.Content.ReadAsStringAsync();

            var countries = JsonConvert.DeserializeObject<List<SelectCountryModel>>(json);
            
            return countries;
        }

        public async Task<Country> GetCountryStats(string countryCode)
        {
            var response = await _client.GetAsync($"{Url(_countryEndpoint)}{countryCode}");
            var json = await response.Content.ReadAsStringAsync();
            
            var country = JsonConvert.DeserializeObject<Country>(json);
            this.CountryStatsLastUpdate = DateTime.Now;

            if (_latestGlobalTotalCases == -1 ||
                _latestGlobalTotalDeaths == -1 ||
                _latestGlobalTotalTests == -1)
                await GetGlobalStats();

            country.TotalGlobalCases = _latestGlobalTotalCases;
            country.TotalGlobalDeaths = _latestGlobalTotalDeaths;
            country.TotalGlobalTests = _latestGlobalTotalTests;

            return country;
        }

        public async Task<CountryHistory> GetCountryHistory(string countryCode, int days = 30)
        {
            var url = Url($"{_countryHistoricalEndpoint}{countryCode}");
            if (days > 0 && days != 30)
            {
                url += $"?lastdays={days}";
            }
            var response = await _client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            var history = JsonConvert.DeserializeObject<CountryHistory>(json);
            this.CountryHistoryLastUpdate = DateTime.Now;

            return history;
        }

        public async Task<List<string>> GetCountryProvinces(string countryCode)
        {
            var history = await GetCountryHistory(countryCode, 1);

            return history.Provinces;
        }

        public async Task<Province> GetProvinceStats(string countryCode, string province, int days = 30)
        {
            var url = Url($"{_countryHistoricalEndpoint}{countryCode}/{province}");
            if (days > 0 && days != 30)
            {
                url += $"?lastdays={days}";
            }
            var response = await _client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            var p = JsonConvert.DeserializeObject<Province>(json);
            this.ProvinceStatsLastUpdate = DateTime.Now;

            if (_latestGlobalTotalCases == -1 ||
                _latestGlobalTotalDeaths == -1)
                await GetGlobalStats();

            p.TotalGlobalCases = _latestGlobalTotalCases;
            p.TotalGlobalDeaths = _latestGlobalTotalDeaths;

            return p;
        }
    }
}
