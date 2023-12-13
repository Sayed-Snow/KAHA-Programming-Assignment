using KAHA.TravelBot.NETCoreReactApp.Models;
using KAHA.TravelBot.NETCoreReactApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace KAHA.TravelBot.NETCoreReactApp.Controllers
{
    [ApiController]
    [Route("/g")]
    public class WeatherForecastController : ControllerBase
    {


        [HttpGet]
        public string Get()
        {
            return "w";
        }
    }
}