using Newtonsoft.Json.Linq;

namespace KAHA.TravelBot.NETCoreReactApp.Models
{
    // TODO: CountrySummaryModel to be implemented
    public class CountrySummaryModel : CountryModel
    {
        public string Name { get; set; }
        public string StartOfWeek { get; set; }
        public string Currency { get; set; }
        public string Flag { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        public int Languages { get; set; }
        public string Timezone { get; set; }
        public string CarSide { get; set; }

    }
}
