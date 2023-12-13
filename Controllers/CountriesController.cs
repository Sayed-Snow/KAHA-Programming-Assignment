using KAHA.TravelBot.NETCoreReactApp.Models;
using KAHA.TravelBot.NETCoreReactApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KAHA.TravelBot.NETCoreReactApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly ITravelBotService _travelBotService;

        public CountriesController( ITravelBotService travelBotService)
        {
            _travelBotService = travelBotService;
        }
        // GET: api/<CountriesController>
        [HttpGet("top5")]
        public async Task<List<CountryModel>> GetTopFive()
        {
            return await _travelBotService.GetTopFiveCountries();
        }
        // POST api/<CountriesController>
        [HttpGet("random")]
        public void GetRandomCountry()
        {
        }
        // GET: api/<CountriesController>/all
        [HttpGet("all")]
        public async Task<List<CountryModel>> GetAllCountries()
        {
            return await _travelBotService.GetAllCountries();
        // GET api/<CountriesController>/Zimbabwe
        }
        [HttpGet("summary/{countryName}")]
        public async Task<CountrySummaryModel> GetCountrySummary(string countryName)
        {
            return await _travelBotService.GetCountrySummary(countryName);
        }
    }
}
