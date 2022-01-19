using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Movies.api.Dto;
using Movies.api.Models;
using Movies.api.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ILogger<MoviesController> _logger;
        private readonly IMovieServices _movieServices;
        private readonly IHttpServices _httpServices;
        private readonly AppSettings _appSettings;

        public MoviesController(IMovieServices movieServices, IHttpServices httpServices, IOptions<AppSettings> appSettings)
        {
            _movieServices = movieServices;
            _httpServices = httpServices;
            _appSettings = appSettings.Value;
        }


        [HttpGet("GetMovieByName")]
        public async Task<IActionResult> GetMovieByName(string name)
        {
            var moviesSearchUrl = $"{_appSettings.ImageUrl}/?t={name}&apikey={_appSettings.ApiKey}";
            var movieApiResponse = _httpServices.Get(moviesSearchUrl);
            var movieResult = JsonConvert.DeserializeObject<MovieDto>(movieApiResponse.data);
            if (movieApiResponse.code == System.Net.HttpStatusCode.OK)
            {
              //for a sucessful response store movies in database
             var response =    await _movieServices.Addmovie(movieResult);
             return Ok(response);
            }
            return BadRequest(
                 new ModelResponse<MovieDto>
                 {
                     IsSucessFull = false,
                     Message = movieApiResponse.data,
                     Payload = null,
                     ResponseCode = movieApiResponse.code
                 }
                );
        }

    }
}
