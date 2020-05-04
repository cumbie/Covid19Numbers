using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Covid19Numbers.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        string _statesEndpoint = "/v2/states/";
        string _statesHistoricalEndpoint = "/v2/historical/usacounties/"; // /{state}

        private int _latestGlobalTotalCases = -1;
        private int _latestGlobalTotalDeaths = -1;
        private int _latestGlobalTotalTests = -1;

        private Dictionary<string, int> _latestCountryTotalCases = new Dictionary<string, int>();
        private Dictionary<string, int> _latestCountryTotalDeaths = new Dictionary<string, int>();
        private Dictionary<string, int> _latestCountryTotalTests = new Dictionary<string, int>();

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
        public DateTime ProvinceHistoryLastUpdate { get; private set; }
        public DateTime StateProvincesStatsLastUpdate { get; private set; }
        public DateTime StateStatsLastUpdate { get; private set; }

        public bool ValidGlobalStats => IsStatValid(GlobalStatsLastUpdate);
        public bool ValidGlobalHistory => IsStatValid(GlobalHistoryLastUpdate);

        public bool ValidCountryStats => IsStatValid(CountryStatsLastUpdate);
        public bool ValidCountryHistory => IsStatValid(CountryHistoryLastUpdate);

        public bool ValidProvinceStats => IsStatValid(ProvinceStatsLastUpdate);
        public bool ValidProvinceHistory => IsStatValid(ProvinceHistoryLastUpdate);
        public bool ValidStateProvincesStats => IsStatValid(StateProvincesStatsLastUpdate);
        public bool ValidStateStats => IsStatValid(StateStatsLastUpdate);

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

            _latestCountryTotalCases[countryCode] = country.Cases;
            _latestCountryTotalDeaths[countryCode] = country.Deaths;
            _latestCountryTotalTests[countryCode] = country.Tests;

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

        public async Task<Province> GetProvinceStats(string countryCode, string province)
        {
            var history = await GetProvinceHistory(countryCode, province, 1000);
            Province p = new Province();
            p.CountryName = history.CountryName;
            p.ProvinceName = history.ProvinceName;
            p.Cases = history.Cases;
            p.Deaths = history.Deaths;
            p.Recovered = history.Recovered;

            this.ProvinceStatsLastUpdate = DateTime.Now;

            if (!_latestCountryTotalCases.ContainsKey(countryCode) ||
                !_latestCountryTotalDeaths.ContainsKey(countryCode))
                await GetCountryStats(countryCode);

            p.TotalCountryCases = _latestCountryTotalCases[countryCode];
            p.TotalCountryDeaths = _latestCountryTotalDeaths[countryCode];

            return p;
        }

        public async Task<ProvinceHistory> GetProvinceHistory(string countryCode, string province, int days = 30)
		{
            var url = Url($"{_countryHistoricalEndpoint}{countryCode}/{province}");
            if (days > 0 && days != 30)
            {
                url += $"?lastdays={days}";
            }
            var response = await _client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            var p = JsonConvert.DeserializeObject<ProvinceHistory>(json);
            this.ProvinceHistoryLastUpdate = DateTime.Now;

            if (!_latestCountryTotalCases.ContainsKey(countryCode) ||
                !_latestCountryTotalDeaths.ContainsKey(countryCode))
                await GetCountryStats(countryCode);

            p.TotalCountryCases = _latestCountryTotalCases[countryCode];
            p.TotalCountryDeaths = _latestCountryTotalDeaths[countryCode];

            return p;
        }

        public async Task<List<string>> GetUsaStates()
        {
            var response = await _client.GetAsync($"{Url(_statesHistoricalEndpoint)}");
            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<string>>(json);
        }

        public async Task<List<ProvinceHistory>> GetUsaStateCountiesAsProvinces(string stateName, int days = 30)
        {
            var url = Url($"{_statesHistoricalEndpoint}{stateName}");
            if (days > 0 && days != 30)
            {
                url += $"?lastdays={days}";
            }
            var response = await _client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            var provinces = JsonConvert.DeserializeObject<List<ProvinceHistory>>(json);

            string countryCode = "us";
            if (!_latestCountryTotalCases.ContainsKey(countryCode) ||
                !_latestCountryTotalDeaths.ContainsKey(countryCode) ||
                !_latestCountryTotalTests.ContainsKey(countryCode))
                await GetCountryStats(countryCode);

            foreach (var province in provinces)
            {
                province.CountryName = countryCode;
                province.TotalCountryCases = _latestCountryTotalCases[countryCode];
                province.TotalCountryDeaths = _latestCountryTotalDeaths[countryCode];
            }

            this.StateProvincesStatsLastUpdate = DateTime.Now;

            return provinces;
        }

        public async Task<ProvinceHistory> GetUsaStateStatsAsProvince(string stateName, int days = 30)
        {
            var stateCounties = await GetUsaStateCountiesAsProvinces(stateName, days);

            string countryCode = "us";

            ProvinceHistory stateProvince = new ProvinceHistory();
            stateProvince.CountryName = countryCode;
            stateProvince.ProvinceName = stateName;
            stateProvince.Timeline = new CountryTimeline();
            foreach (var stateCounty in stateCounties)
            {
                foreach (var kvp in stateCounty.Timeline.Cases)
                {
                    if (!stateProvince.Timeline.Cases.ContainsKey(kvp.Key))
                        stateProvince.Timeline.Cases.Add(kvp.Key, kvp.Value);
                    else
                        stateProvince.Timeline.Cases[kvp.Key] += kvp.Value;
                }
                foreach (var kvp in stateCounty.Timeline.Deaths)
                {
                    if (!stateProvince.Timeline.Deaths.ContainsKey(kvp.Key))
                        stateProvince.Timeline.Deaths.Add(kvp.Key, kvp.Value);
                    else
                        stateProvince.Timeline.Deaths[kvp.Key] += kvp.Value;
                }
                foreach (var kvp in stateCounty.Timeline.Recovered)
                {
                    if (!stateProvince.Timeline.Recovered.ContainsKey(kvp.Key))
                        stateProvince.Timeline.Recovered.Add(kvp.Key, kvp.Value);
                    else
                        stateProvince.Timeline.Recovered[kvp.Key] += kvp.Value;
                }
            }

            if (!_latestCountryTotalCases.ContainsKey(countryCode) ||
                !_latestCountryTotalDeaths.ContainsKey(countryCode) ||
                !_latestCountryTotalTests.ContainsKey(countryCode))
                await GetCountryStats(countryCode);

            stateProvince.TotalCountryCases = _latestCountryTotalCases[countryCode];
            stateProvince.TotalCountryDeaths = _latestCountryTotalDeaths[countryCode];

            return stateProvince;
        }

        public async Task<List<Province>> GetAllUsaStateStatsAsProvince()
		{
            var states = await GetAllUsaStateStats();
            List<Province> provinces = new List<Province>();

            string countryCode = "us";
            if (!_latestCountryTotalCases.ContainsKey(countryCode) ||
                !_latestCountryTotalDeaths.ContainsKey(countryCode) ||
                !_latestCountryTotalTests.ContainsKey(countryCode))
                await GetCountryStats(countryCode);

            foreach (var state in states)
            {
                var province = new Province
                {
                    CountryName = "US",
                    ProvinceName = state.StateName,
                    Cases = state.Cases,
                    Deaths = state.Deaths,
                    Recovered = state.Recovered
                };

                province.TotalCountryCases = _latestCountryTotalCases[countryCode];
                province.TotalCountryDeaths = _latestCountryTotalDeaths[countryCode];
            }

            return provinces;
		}

        public async Task<List<State>> GetAllUsaStateStats()
        {
            var response = await _client.GetAsync($"{Url(_statesEndpoint)}");
            var json = await response.Content.ReadAsStringAsync();

            var states = JsonConvert.DeserializeObject<List<State>>(json);
            this.StateStatsLastUpdate = DateTime.Now;

            string countryCode = "us";
            if (!_latestCountryTotalCases.ContainsKey(countryCode) ||
                !_latestCountryTotalDeaths.ContainsKey(countryCode) ||
                !_latestCountryTotalTests.ContainsKey(countryCode))
                await GetCountryStats(countryCode);

            foreach (var state in states)
            {
                state.TotalCountryCases = _latestCountryTotalCases[countryCode];
                state.TotalCountryDeaths = _latestCountryTotalDeaths[countryCode];
                state.TotalCountryTests = _latestCountryTotalTests[countryCode];
            }

            return states;
        }
    }
}
