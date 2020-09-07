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
        string _worldEndpoint = "/v3/covid-19/all";
        string _continentsEndpoint = "/v3/covid-19/continents";
        string _worldHistorialEndpoint = "/v3/covid-19/historical/all";

        // country
        string _countryEndpoint = "/v3/covid-19/countries/";
        string _countryHistoricalEndpoint = "/v3/covid-19/historical/"; // /{country}/{province}

        // states/provinces
        string _statesEndpoint = "/v3/covid-19/states/";
        string _statesHistoricalEndpoint = "/v3/covid-19/historical/usacounties/"; // /{state}

        private int _latestGlobalTotalCases = -1;
        private int _latestGlobalTotalDeaths = -1;
        private int _latestGlobalTotalRecovered = -1;
        private int _latestGlobalTotalTests = -1;

        private Dictionary<string, int> _latestCountryTotalCases = new Dictionary<string, int>();
        private Dictionary<string, int> _latestCountryTotalDeaths = new Dictionary<string, int>();
        private Dictionary<string, int> _latestCountryTotalRecovered = new Dictionary<string, int>();
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
            _latestGlobalTotalRecovered = world.Recovered;
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
                _latestGlobalTotalRecovered == -1 ||
                _latestGlobalTotalTests == -1)
                await GetGlobalStats();

            country.TotalGlobalCases = _latestGlobalTotalCases;
            country.TotalGlobalDeaths = _latestGlobalTotalDeaths;
            country.TotalGlobalRecovered = _latestGlobalTotalRecovered;
            country.TotalGlobalTests = _latestGlobalTotalTests;

            _latestCountryTotalCases[countryCode] = country.Cases;
            _latestCountryTotalDeaths[countryCode] = country.Deaths;
            _latestCountryTotalRecovered[countryCode] = country.Recovered;
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

            if (!_latestCountryTotalCases.ContainsKey(countryCode) ||
                !_latestCountryTotalDeaths.ContainsKey(countryCode) ||
                !_latestCountryTotalRecovered.ContainsKey(countryCode))
                await GetCountryStats(countryCode);

            history.TotalCountryCases = _latestCountryTotalCases[countryCode];
            history.TotalCountryDeaths = _latestCountryTotalDeaths[countryCode];
            history.TotalCountryRecovered = _latestCountryTotalRecovered[countryCode];

            return history;
        }

        public async Task<List<string>> GetCountryProvinces(string countryCode)
        {
            var history = await GetCountryHistory(countryCode, 1);

            return history.Provinces;
        }

        public async Task<Province> GetProvinceStats(string countryCode, string province)
        {
            Province p = new Province();

            var histories = await GetProvinceHistory(countryCode, province, 1000);
            var history = histories?.First();

            if (history != null)
            {
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
            }

            return p;
        }

        public async Task<List<ProvinceHistory>> GetProvinceHistory(string countryCode, string province, int days = 30)
		{
            List<ProvinceHistory> histories = new List<ProvinceHistory>();

            if (!countryCode.ToLower().StartsWith("us"))
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

                histories.Add(p);
            }
            else
            {
                var url = Url($"{_statesHistoricalEndpoint}{province}");
                if (days > 0 && days != 30)
                {
                    url += $"?lastdays={days}";
                }
                var response = await _client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                histories = JsonConvert.DeserializeObject<List<ProvinceHistory>>(json);

                if (!_latestCountryTotalCases.ContainsKey(countryCode) ||
                    !_latestCountryTotalDeaths.ContainsKey(countryCode))
                    await GetCountryStats(countryCode);

                foreach (var history in histories)
                {
                    history.CountryName = countryCode;
                    history.TotalCountryCases = _latestCountryTotalCases[countryCode];
                    history.TotalCountryDeaths = _latestCountryTotalDeaths[countryCode];
                }

                this.StateProvincesStatsLastUpdate = DateTime.Now;
            }

            return histories;
        }

        public async Task<List<string>> GetUsaStates()
        {
            // combine list from _statesHistoricalEndpoint and _statesEndpoint
            // and then add checks to other APIs to call other endpoints to get correct data

            // from historical
            var response = await _client.GetAsync($"{Url(_statesHistoricalEndpoint)}");
            var json = await response.Content.ReadAsStringAsync();
            var historyStates = JsonConvert.DeserializeObject<List<string>>(json).ToList();

            // from states
            response = await _client.GetAsync($"{Url(_statesEndpoint)}");
            json = await response.Content.ReadAsStringAsync();
            var states = JsonConvert.DeserializeObject<List<State>>(json);
            var stateNames = states.Select(s => s.StateName.ToLower()).ToList();

            historyStates.AddRange(stateNames);

            var aggregateStateNames = historyStates.Distinct().ToList();
            aggregateStateNames.Sort();

            return aggregateStateNames;
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
                !_latestCountryTotalDeaths.ContainsKey(countryCode))
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

        public async Task<Province> GetUsaStateStatsAsProvince(string stateName)
        {
            Province province = null;

            string countryCode = "us";
            if (!_latestCountryTotalCases.ContainsKey(countryCode) ||
                !_latestCountryTotalDeaths.ContainsKey(countryCode) ||
                !_latestCountryTotalTests.ContainsKey(countryCode))
                await GetCountryStats(countryCode);

            var response = await _client.GetAsync(Url($"{_statesEndpoint}{stateName}"));
            var json = await response.Content.ReadAsStringAsync();

            try
            {
                // try state endpoint first
                var state = JsonConvert.DeserializeObject<State>(json);
                if (string.IsNullOrWhiteSpace(state.Message)) // stateName found (no error)
                {
                    province = new Province
                    {
                        CountryName = countryCode,
                        ProvinceName = stateName,
                        Cases = state.Cases,
                        Deaths = state.Deaths,
                        Recovered = state.Recovered,
                        TotalCountryCases = _latestCountryTotalCases[countryCode],
                        TotalCountryDeaths = _latestCountryTotalDeaths[countryCode]
                    };
                }
                else
                {
                    // not found, try other endpoint
                    Province tmpProvince = new Province
                    {
                        CountryName = countryCode,
                        ProvinceName = stateName,
                        TotalCountryCases = _latestCountryTotalCases[countryCode],
                        TotalCountryDeaths = _latestCountryTotalDeaths[countryCode]
                    };
                    
                    var stateCounties = await GetUsaStateCountiesAsProvinces(stateName, 1);
                    foreach (var stateCounty in stateCounties)
                    {
                        tmpProvince.Cases += stateCounty.Cases;
                        tmpProvince.Deaths += stateCounty.Deaths;
                        tmpProvince.Recovered += stateCounty.Recovered;
                    }

                    province = tmpProvince;
                }
            }
            catch (Exception x)
            {

            }

            return province;
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

                provinces.Add(province);
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
