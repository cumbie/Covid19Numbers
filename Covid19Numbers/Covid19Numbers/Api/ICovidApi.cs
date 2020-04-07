using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Covid19Numbers.Models;

namespace Covid19Numbers.Api
{
    public interface ICovidApi
    {
        Task<World> GetGlobalStats();
        Task<List<SelectCountryModel>> GetCountryList();
        Task<Country> GetCountryStats(string countryCode);
    }
}
