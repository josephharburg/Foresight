using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Foresight.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Foresight.Controllers
{
    public class APIController : Controller
    {
        private IConfiguration _config;
        public APIController(IConfiguration config)
        {
            _config = config;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<Weather> GetWeather(string city)
        {
            var weatherApiKey = _config["weatherApiKey"];
            Weather w = new Weather();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"https://api.airvisual.com/v2/city?city={city}&state=Michigan&country=USA&key={weatherApiKey}"))
                {
                    var sr = await response.Content.ReadAsStringAsync();
                    JsonDocument jdoc = JsonDocument.Parse(sr);
                    var current = jdoc.RootElement.GetProperty("data").GetProperty("current");
                    w.temperature = current.GetProperty("weather").GetProperty("tp").GetDouble();
                    w.humidity = current.GetProperty("weather").GetProperty("hu").GetDouble();
                    w.windspeed = current.GetProperty("weather").GetProperty("ws").GetDouble();
                    w.aqi = current.GetProperty("pollution").GetProperty("aqius").GetDouble();
                    w.weather = current.GetProperty("weather").GetProperty("ic").GetString();
                }
            }
            return w;
        }
        
        public async Task<List<News>> GetNews()
        {
            var newsapikey = _config["newsapikey"];
            List<News> articlelist = new List<News>();
            string fear = "coronavirus";
            using (var httpclient = new HttpClient())
            {
                using (var response = await httpclient.GetAsync($"http://newsapi.org/v2/top-headlines?q={fear}&apiKey={newsapikey}"))
                {
                    var sr = await response.Content.ReadAsStringAsync();
                    JsonDocument jdoc = JsonDocument.Parse(sr);
                    var articles = jdoc.RootElement.GetProperty("articles").EnumerateArray();
                    var list = articles.ToArray<JsonElement>();

                    for (int x = 0; x < 4; x++)
                    {
                        articlelist.Add(new News() { title = list[x].GetProperty("title").GetString(), description = list[x].GetProperty("description").GetString(), content = list[x].GetProperty("content").GetString(), url = list[x].GetProperty("url").GetString(), urlToImage = list[x].GetProperty("urlToImage").GetString() });
                    }

                }
            }
            return articlelist;
        }
    }
}