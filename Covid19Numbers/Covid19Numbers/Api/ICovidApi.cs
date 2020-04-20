using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Covid19Numbers.Models;

namespace Covid19Numbers.Api
{
    public interface ICovidApi
    {
        DateTime GlobalHistoryLastUpdate { get; }
        DateTime GlobalStatsLastUpdate { get; }
        DateTime CountryStatsLastUpdate { get; }

        Task<World> GetGlobalStats();

        Task<WorldHistory> GetGlobalHistory(int days = 30);

        Task<List<SelectCountryModel>> GetCountryList();

        Task<Country> GetCountryStats(string countryCode);
    }
}
