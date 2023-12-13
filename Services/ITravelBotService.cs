using KAHA.TravelBot.NETCoreReactApp.Models;

namespace KAHA.TravelBot.NETCoreReactApp.Services
{
    public interface ITravelBotService
    {
        public Task<List<CountryModel>> GetTopFiveCountries();
        public Task<List<CountryModel>> GetAllCountries();
        public Task<(string, string)> GetSunriseSunsetTimes(string countryName);
        public Task<CountrySummaryModel> GetCountrySummary(string countryName);


    }
}
