using AutoMapper;
using Movies.api.Dto;
using Movies.api.Models;

namespace BlazorTemplate.api.Profiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<MovieDto, Movie>();
            CreateMap<Movie, MovieDto>();

            CreateMap<RatingDto, Rating>();
            CreateMap<Rating, RatingDto>();

        }


    }
}
