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

        public LmaoNinjaCovidApi()
        {
            _client = new HttpClient();
        }

        private string Url(string endpoint) => $"{_baseUrl}{endpoint}";

        public async Task<WorldHistory> GetGlobalHistory(int days = 30) // TODO: pulldown to select days
        {
            var response = await _client.GetAsync(Url(_worldHistorialEndpoint));
            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<WorldHistory>(json);
        }

        public async Task<World> GetGlobalStats()
        {
            var response = await _client.GetAsync(Url(_worldEndpoint));
            var json = await response.Content.ReadAsStringAsync();

            var world = JsonConvert.DeserializeObject<World>(json);
            _latestGlobalTotalCases = world.Cases;
            _latestGlobalTotalDeaths = world.Deaths;

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
            if (_latestGlobalTotalCases == -1 || _latestGlobalTotalDeaths == -1)
                await GetGlobalStats();

            country.TotalGlobalCases = _latestGlobalTotalCases;
            country.TotalGlobalDeaths = _latestGlobalTotalDeaths;
            return country;
        }
    }
}
