using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Covid19Numbers.Models;

namespace Covid19Numbers.Api
{
    public interface ICovidApi
    {
        bool ValidGlobalStats { get; }
        bool ValidGlobalHistory { get; }

        bool ValidCountryStats { get; }
        bool ValidCountryHistory { get; }

        bool ValidProvinceStats { get; }

        Task<World> GetGlobalStats();

        Task<WorldHistory> GetGlobalHistory(int days = 30);

        Task<List<SelectCountryModel>> GetCountryList();

        Task<Country> GetCountryStats(string countryCode);

        Task<CountryHistory> GetCountryHistory(string countryCode, int days = 30);

        Task<List<string>> GetCountryProvinces(string countryCode);

        Task<Province> GetProvinceStats(string countryCode, string province, int days = 30);
    }
}
