using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Movies.api.Data;
using Movies.api.Dto;
using Movies.api.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.api.Services
{
    public interface IMovieServices
    {
    
        Task<ModelResponse<MovieDto>> Addmovie(MovieDto movieDto);
        Task<ModelResponse<List<MovieDto>>> GetAllMovies();
        ModelResponse<List<MovieDto>> SearchMovies(string movieName);
         ModelResponse<MovieDto> GetMovieById(string Id);
    }

    public class MovieServices :  IMovieServices
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IApplicationWriteDbConnection _writeDbConnection;
        private readonly IMapper _mapper;
        public MovieServices(ApplicationDbContext dbContext, IApplicationWriteDbConnection writeDbConnection, IMapper mapper)
        {
            _dbContext = dbContext;
            _writeDbConnection = writeDbConnection;
            _mapper = mapper;
        }

        public async Task<ModelResponse<MovieDto>> Addmovie(MovieDto movieDto)
        {
            _dbContext.Connection.Open();
            using (var transaction = _dbContext.Connection.BeginTransaction())
            {
                try
                {
                    _dbContext.Database.UseTransaction(transaction as DbTransaction);
                    //Check if cat already exist
                    bool moviesExist = await _dbContext.Movies.AnyAsync(a => a.imdbID == movieDto.imdbID);
                    if (moviesExist)
                    {
                        throw new Exception("Movies Already Exists");
                    }
                    //Add new movie
                    var movieToInsert = _mapper.Map<Movie>(movieDto);
                    _dbContext.Movies.Add(movieToInsert);
                    var movieSaveResult = await _dbContext.SaveChangesAsync(default);
                    if (movieSaveResult == 0)
                    {
                        throw new Exception("Failed to save movie");

                    }
                    transaction.Commit();
                    var movieDtoResponse = _mapper.Map<MovieDto>(movieToInsert);
                    return new ModelResponse<MovieDto>
                    {
                        IsSucessFull = true,
                        Payload = movieDtoResponse,
                        ResponseCode = System.Net.HttpStatusCode.OK,
                        Message = "Movies added sucessful"
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                finally
                {
                    _dbContext.Connection.Close();
                }
            }
        }

        public ModelResponse<List<MovieDto>> SearchMovies(string movieName)
        {
            var moviesFromDB = _dbContext.Movies.Include(x=>x.Ratings)
                .Where(x => x.Title.ToLower().Trim() == movieName.ToLower().Trim()).ToList();
            var moviesToReturn = _mapper.Map<List<MovieDto>>(moviesFromDB);
            return new ModelResponse<List<MovieDto>>
            {
                IsSucessFull = true,
                Message = "Sucessful",
                Payload = moviesToReturn,
                ResponseCode = System.Net.HttpStatusCode.OK
            };
        }

        public ModelResponse<MovieDto> GetMovieById(string Id)
        {
            var movieFromDB = _dbContext.Movies.Include(x => x.Ratings)
                .FirstOrDefault(x => x.imdbID.ToLower().Trim() == Id.ToLower().Trim());
            var movieToReturn = _mapper.Map<MovieDto>(movieFromDB);
            return new ModelResponse<MovieDto>
            {
                IsSucessFull = true,
                Message = "Sucessful",
                Payload = movieToReturn,
                ResponseCode = System.Net.HttpStatusCode.OK
            };
        }


        

        public async Task<ModelResponse<List<MovieDto>>> GetAllMovies()
        {
            var moviesFromDB = await _dbContext.Movies.Include(x=>x.Ratings).ToListAsync();
            var moviesToReturn = _mapper.Map<List<MovieDto>>(moviesFromDB);
            return new ModelResponse<List<MovieDto>>
            {
                IsSucessFull = true,
                Message = "Sucessful",
                Payload = moviesToReturn,
                ResponseCode = System.Net.HttpStatusCode.OK
            };
        }

    }
}
