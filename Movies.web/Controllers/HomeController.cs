using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Movies.api.Models;
using Movies.web.Models;
using Movies.web.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IHttpServices _httpServices;
        private readonly AppSettings _appSettings;

        public HomeController(ILogger<HomeController> logger, IHttpServices httpServices, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _httpServices = httpServices;
            _appSettings = appSettings.Value;

        }

        public IActionResult Index()
        {
            //get all existing images from database
            string allMoviesUrl = $"{_appSettings.ApiUrl}/GetAllMovies";
            var allMoviesResponse = _httpServices.Get(allMoviesUrl);
            var allMoviesResult = JsonConvert.DeserializeObject<ModelResponse<List<Movie>>>(allMoviesResponse.data);
            return View(allMoviesResult.Payload);
        }

        public IActionResult Details(string Id)
        {
            //get all existing images from database
            string allMoviesUrl = $"{_appSettings.ApiUrl}/GetMovieById?Id={Id}";
            var allMoviesResponse = _httpServices.Get(allMoviesUrl);
            var allMoviesResult = JsonConvert.DeserializeObject<ModelResponse<Movie>>(allMoviesResponse.data);
            return View(allMoviesResult.Payload);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
