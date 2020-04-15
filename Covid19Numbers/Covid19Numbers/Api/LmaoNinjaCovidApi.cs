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
        string _worldHistorialEndpoint = "/v2/historical/all";
        string _countryEndpoint = "/v2/countries/";
        string _statesEndpoint = "/states";

        public LmaoNinjaCovidApi()
        {
            _client = new HttpClient();
        }

        private string Url(string endpoint) => $"{_baseUrl}{endpoint}";

        public async Task<WorldHistory> GetGlobalHistory(int days = 30)
        {
            var response = await _client.GetAsync(Url(_worldHistorialEndpoint));
            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<WorldHistory>(json);
        }

        public async Task<World> GetGlobalStats()
        {
            var response = await _client.GetAsync(Url(_worldEndpoint));
            //var json = "{\"cases\":597458,\"deaths\":27370,\"recovered\":133373,\"updated\":1585375470343,\"active\":436715}";
            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<World>(json);
        }

        public async Task<List<SelectCountryModel>> GetCountryList()
        {
            var response = await _client.GetAsync($"{Url(_countryEndpoint)}");
            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<SelectCountryModel>>(json);
        }

        public async Task<Country> GetCountryStats(string countryCode)
        {
            var response = await _client.GetAsync($"{Url(_countryEndpoint)}{countryCode}");
            var json = await response.Content.ReadAsStringAsync();
            //var json = "{'country':'USA','countryInfo':{'_id':840,'lat':38,'long':-97,'flag':'https://raw.githubusercontent.com/NovelCOVID/API/master/assets/flags/us.png','iso3':'USA','iso2':'US'},'cases':112815,'todayCases':8689,'deaths':1880,'todayDeaths':184,'recovered':3219,'active':107716,'critical':2666,'casesPerOneMillion':341,'deathsPerOneMillion':6}";

            return JsonConvert.DeserializeObject<Country>(json);
        }
    }
}
