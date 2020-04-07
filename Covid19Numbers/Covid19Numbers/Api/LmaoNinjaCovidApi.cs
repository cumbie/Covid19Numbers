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

        string _worldUrl = "https://corona.lmao.ninja/all";
        string _countryUrl = "https://corona.lmao.ninja/countries/";
        string _statesUrl = "https://corona.lmao.ninja/states";

        public LmaoNinjaCovidApi()
        {
            _client = new HttpClient();
        }

        public async Task<List<SelectCountryModel>> GetCountryList()
        {
            var resp = await _client.GetAsync($"{_countryUrl}");
            var json = await resp.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<SelectCountryModel>>(json);
        }

        public async Task<Country> GetCountryStats(string countryCode)
        {
            var countryResponse = await _client.GetAsync($"{_countryUrl}{countryCode}");
            var countryJson = await countryResponse.Content.ReadAsStringAsync();
            //var countryJson = "{'country':'USA','countryInfo':{'_id':840,'lat':38,'long':-97,'flag':'https://raw.githubusercontent.com/NovelCOVID/API/master/assets/flags/us.png','iso3':'USA','iso2':'US'},'cases':112815,'todayCases':8689,'deaths':1880,'todayDeaths':184,'recovered':3219,'active':107716,'critical':2666,'casesPerOneMillion':341,'deathsPerOneMillion':6}";

            return JsonConvert.DeserializeObject<Country>(countryJson);
        }

        public async Task<World> GetGlobalStats()
        {
            var worldResponse = await _client.GetAsync(_worldUrl);
            //var worldson = "{\"cases\":597458,\"deaths\":27370,\"recovered\":133373,\"updated\":1585375470343,\"active\":436715}";
            var worldson = await worldResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<World>(worldson);
        }
    }
}
