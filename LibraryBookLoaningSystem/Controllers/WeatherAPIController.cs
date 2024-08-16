using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using System.Web.Http.Results;
using LibraryBookLoaningSystem.ViewModels;
using System.Net;
using System.Web.Helpers;
using LibraryBookLoaningSystem.Helpers;
using LibraryBookLoaningSystem.Models;
using System.IO;
using System.Text;

namespace LibraryBookLoaningSystem.Controllers
{
    //[Microsoft.AspNetCore.Components.Route("[controller]")]
    public class WeatherAPIController : Controller
    {
        private readonly string _apiKey = "0e2432c422b08864dd50d8d8b76080ee";

        public WeatherAPIController()
        {
        }

        public ActionResult Index()
        {
            WeatherAPI weatherAPI = AddCities();
            return View(weatherAPI);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<IActionResult> Index(WeatherAPI weatherAPI, string cities)
        {
            weatherAPI = AddCities();
            if (cities != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://api.openweathermap.org");
                    var response = await client.GetAsync($"/data/2.5/weather?id={cities}&appid={_apiKey}&units=metric");
                    //response.EnsureSuccessStatusCode();
                    var stringResult = await response.Content.ReadAsStringAsync();
                    var rawWeather = JsonConvert.DeserializeObject<WeatherData.ResponseWeather>(stringResult);

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<table><tr><th>Weather Description</th></tr>");
                    sb.Append("<tr><td>City:</td><td>" +
                    rawWeather.name + "</td></tr>");
                    //sb.Append("<tr><td>Country:</td><td>" +
                    //rawWeather.sys.country + "</td></tr>");
                    sb.Append("<tr><td>Wind:</td><td>" +
                    rawWeather.wind.speed + " mph</td></tr>");
                    sb.Append("<tr><td>Current Temperature:</td><td>" +
                    rawWeather.main.temp + " °C</td></tr>");
                    sb.Append("<tr><td>Humidity:</td><td>" +
                    rawWeather.main.humidity + "</td></tr>");
                    sb.Append("<tr><td>Weather:</td><td>" +
                    rawWeather.weather[0].description + "</td></tr>");
                    sb.Append("</table>");
                    weatherAPI.apiResponse = sb.ToString();
                }
            }
            return View(weatherAPI);
        }
        public WeatherAPI AddCities()
        {
            WeatherAPI weatherAPI = new WeatherAPI();
            weatherAPI.cities = new Dictionary<string, string>();
            weatherAPI.cities.Add("London", "2643743");
            weatherAPI.cities.Add("Hong Kong", "1819729");
            weatherAPI.cities.Add("Tokyo", "1850147");
            weatherAPI.cities.Add("New York", "5128581");
            weatherAPI.cities.Add("Lisbon", "2267057");
            return weatherAPI;
        }
    }
}
