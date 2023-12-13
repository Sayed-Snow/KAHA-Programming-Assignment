using System;
using KAHA.TravelBot.NETCoreReactApp.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Abstractions;

namespace KAHA.TravelBot.NETCoreReactApp.Services
{
    public class TravelBotService : ITravelBotService
    {
        public List<CountryModel> Countries { get; set; }

        public async Task<List<CountryModel>> GetAllCountries()
        {
            using (var httpClient = new HttpClient())
            {
                var apiUrl = "https://restcountries.com/v3.1/all?fields=name,population,capital,capitalInfo";

                var response = await httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error getting data: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var parsedResponse = JArray.Parse(content);

                var countries = new List<CountryModel>();

                foreach (var item in parsedResponse)
                {
                    var country = new CountryModel
                    {
                        Name = item["name"]?["common"]?.ToString(),
                        Capital = item["capital"]?.FirstOrDefault()?.ToString(),
                        Population = (int)(item["population"] ?? 0),
                        Latitude = TryParseLatitude(item),
                        Longitude = TryParseLongitude(item)
                    };
                    countries.Add(country);

                }

                return countries;
            }
        }

        private float TryParseLatitude(JToken item)
        {
            if (item["capitalInfo"]?["latlng"]?.Count() != 2)
            {
                return 0f; // Handle missing latitude
            }

            try
            {
                return float.Parse(item["capitalInfo"]["latlng"][0].ToString());
            }
            catch (Exception)
            {
                return 0f; // Handle invalid latitude format
            }
        }

        private float TryParseLongitude(JToken item)
        {
            if (item["capitalInfo"]?["latlng"]?.Count() != 2)
            {
                return 0f; // Handle missing longitude
            }

            try
            {
                return float.Parse(item["capitalInfo"]["latlng"][1].ToString());
            }
            catch (Exception)
            {
                return 0f; // Handle invalid longitude format
            }
        }

        // Top 5 Countries by population size
        public async Task<List<CountryModel>> GetTopFiveCountries()
        {
            using var httpClient = new HttpClient();
            const string apiUrl = "https://restcountries.com/v3.1/all?fields=name,population,capital,capitalInfo";

            try
            {
                using var response = await httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error getting data: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var parsedResponse = JArray.Parse(content);

                var southernHemisphereCountries = new List<CountryModel>();

                foreach (var item in parsedResponse)
                {
                    try
                    {
                        var latitude = TryParseLatitude(item);
                        if (latitude < 0) // Check for latitude in southern hemisphere
                        {
                            southernHemisphereCountries.Add(new CountryModel
                            {
                                Name = item["name"]?["common"]?.ToString(),
                                Population = (int)(item["population"] ?? 0),
                                Capital = item["capital"]?.FirstOrDefault()?.ToString(),
                                Latitude = latitude,
                                Longitude = TryParseLongitude(item)
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing country: {ex.Message}");
                        Console.WriteLine(ex.ToString());
                    }
                }

                southernHemisphereCountries.Sort((x, y) => y.Population.CompareTo(x.Population));
                return southernHemisphereCountries.Take(5).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.ToString());

                throw new Exception($"Error getting data: {ex.Message}", ex);
            }
        }

        public async Task<CountrySummaryModel> GetCountrySummary(string countryName)
        {
            var sunriseSunsetTimes = await GetSunriseSunsetTimes(countryName); 

            using var httpClient = new HttpClient();
            var countryDetailsUrl = $"https://restcountries.com/v3.1/name/{countryName}?fields=name,flags,currencies,latlng,timezones,car,population,capital,startOfWeek,languages";

            using (var response = await httpClient.GetAsync(countryDetailsUrl))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error getting country details: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var parsedResponse = JArray.Parse(content);

                if (parsedResponse.Count != 1)
                {
                    throw new Exception($"Invalid response received for country '{countryName}'.");
                }

                var countryDetails = parsedResponse[0];
                
                var summary = new CountrySummaryModel
                {
                    Name = countryDetails["name"]["common"]?.ToString(),
                    StartOfWeek = countryDetails["startOfWeek"]?.ToString(),
                    Capital = countryDetails["capital"][0].ToString(),
                    Currency = countryDetails["currencies"].Values().First()["name"].ToString(),
                    Flag = countryDetails["flags"]["svg"]?.ToString(),
                    Sunrise = sunriseSunsetTimes.Item1, 
                    Sunset = sunriseSunsetTimes.Item2,
                    Languages = countryDetails["languages"]?.Count() ?? 0,
                    Population = (int)(countryDetails["population"] ?? 0),
                    Timezone = countryDetails["timezones"][0].ToString(),
                    CarSide = countryDetails["car"]["side"]?.ToString()?.ToLower(),
                    Latitude = (float)countryDetails["latlng"][0],
                    Longitude = (float)countryDetails["latlng"][1]

                };

                return summary;
            }
        }


        public CountryModel RandomCountryInSouthernHemisphere(List<CountryModel> countries)
        {
            // Subtle bug in code
            var countriesInSouthernHemisphere = countries.Where(x => x.Latitude < 0);
            var random = new Random();
            var randomIndex = random.Next(0, countriesInSouthernHemisphere.Count());
            return countriesInSouthernHemisphere.ElementAt(randomIndex);
        }

        public async Task<(string, string)> GetSunriseSunsetTimes(string countryName)
        {
            var (latitude, longitude) = await GetLatLong(countryName);
            var apiUrl = $"https://api.sunrise-sunset.org/json?lat={latitude}&lng={-longitude}";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    using (var response = await httpClient.GetAsync(apiUrl))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"Error getting data: {response.StatusCode}");
                        }

                        var content = await response.Content.ReadAsStringAsync();
                        var parsedResponse = JObject.Parse(content);

                        var results = parsedResponse["results"];
                        var sunrise = results["sunrise"].ToString();
                        var sunset = results["sunset"].ToString();

                        return (sunrise, sunset);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw new Exception($"Error getting sunrise/sunset for '{countryName}': {ex.Message}", ex);
                }
            }
        }


        public async Task<(double latitude, double longitude)> GetLatLong(string countryName)
        {
            using var httpClient = new HttpClient();
            var apiUrl = $"https://restcountries.com/v3.1/name/{countryName}?fields=latlng";

            try
            {
                using var response = await httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error getting data: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var parsedResponse = JArray.Parse(content);
                var latlng = parsedResponse[0]["latlng"];

                return ((double)latlng[0], (double)latlng[1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception($"Error getting lat/long for '{countryName}': {ex.Message}", ex);
            }
        }
    }
}
