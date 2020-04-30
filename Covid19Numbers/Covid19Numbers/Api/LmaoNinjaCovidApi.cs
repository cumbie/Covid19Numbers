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

        string _baseUrl = "https://corona.lmao.ninja";
        string _worldEndpoint = "/v2/all";
        string _continentsEndpoint = "/v2/continents";
        string _worldHistorialEndpoint = "/v2/historical/all";

        string _countryEndpoint = "/v2/countries/";

        string _statesEndpoint = "/states";

        private int _latestGlobalTotalCases = -1;
        private int _latestGlobalTotalDeaths = -1;
        private int _latestGlobalTotalTests = -1;

        public LmaoNinjaCovidApi()
        {
            _client = new HttpClient();
        }

        #region Properties

        public DateTime GlobalHistoryLastUpdate { get; private set; }
        public DateTime GlobalStatsLastUpdate { get; private set; }
        public DateTime CountryStatsLastUpdate { get; private set; }

        #endregion

        private string Url(string endpoint) => $"{_baseUrl}{endpoint}";

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
    }
}
