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
        Task<ModelResponse<MovieDto>> GetMovieByName(string name);

        Task<ModelResponse<MovieDto>> Addmovie(MovieDto movieDto);

        Task<ModelResponse<MovieDto>> GetMovieByNameFromLocalRepo(string name);

        Task<ModelResponse<IEnumerable<MovieDto>>> GetAllMoviesFromLocalRepo();
    }
    public class MovieServices : IMovieServices
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IApplicationWriteDbConnection _writeDbConnection;
        public IMapper _mapper { get; set; }
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
                    bool moviesExist = await _dbContext.Movies.AnyAsync(a => a.Title == movieDto.Title);
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

                    //add ratings for movie

                    _dbContext.Ratings.AddRange(movieToInsert.Ratings);
                    var imageSaveResult = await _dbContext.SaveChangesAsync(default);


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

        public Task<ModelResponse<IEnumerable<MovieDto>>> GetAllMoviesFromLocalRepo()
        {
            throw new NotImplementedException();
        }

        public Task<ModelResponse<MovieDto>> GetMovieByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<ModelResponse<MovieDto>> GetMovieByNameFromLocalRepo(string name)
        {
            throw new NotImplementedException();
        }
    }
}
