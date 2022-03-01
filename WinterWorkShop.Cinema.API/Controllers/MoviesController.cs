using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        private readonly ILogger<MoviesController> _logger;

        public MoviesController(ILogger<MoviesController> logger, IMovieService movieService)
        {
            _logger = logger;
            _movieService = movieService;
        }

        [HttpGet]
        [Route("auditoriumId/{id}")]
        public async Task<ActionResult<ResponseModel<IEnumerable<MovieDomainModel>>>> GetMoviesByAuditoriumId(int id)
        {
            var movies = await _movieService.GetMoviesByAuditoriumId(id);
            if (!movies.IsSuccessful)
                return NotFound(new ErrorResponseModel()
                {
                    ErrorMessage = movies.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.NotFound
                });

            return Ok(movies.DomainModel);
        }

        /// <summary>
        /// Gets Movie by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ResponseModel<MovieDomainModel>>> GetAsync(Guid id)
        {
            ResponseModel<MovieDomainModel> movie;

            movie = await _movieService.GetMovieByIdAsync(id);

            if (movie.IsSuccessful == false)
            {
                return NotFound(movie.ErrorMessage);
            }

            return Ok(movie.DomainModel);
        }

        /// <summary>
        /// Gets all current movies
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("current")]
        public ActionResult<IEnumerable<MovieDomainModel>> GetCurrent()
        {
            IEnumerable<MovieDomainModel> movieDomainModels;

            movieDomainModels = _movieService.GetCurrentMovies(true);

            if (movieDomainModels == null)
            {
                movieDomainModels = new List<MovieDomainModel>();
            }

            return Ok(movieDomainModels);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDomainModel>>> GetAll()
        {
            IEnumerable<MovieDomainModel> movieDomainModels;

            movieDomainModels = await _movieService.GetAllMovies();

            if (movieDomainModels == null)
            {
                movieDomainModels = new List<MovieDomainModel>();
            }

            return Ok(movieDomainModels);
        }

        [HttpGet]
        [Route("top10")]

        public async Task<ActionResult<IEnumerable<MovieDomainModel>>> GetTopTenMovies()
        {
            var result = await _movieService.GetTopTenMovies();
            if (result == null)
            {
                result = new List<MovieDomainModel>();
            }
            return Ok(result);
        }


        [HttpGet]
        [Route("top10/{year}")]
        public async Task<ActionResult<IEnumerable<MovieDomainModel>>> GetTopTenMovies(int year)
        {

            var result = await _movieService.GetTopTenMoviesByYear(year);
            if (result == null)
            {
                result = new List<MovieDomainModel>();
            }
            return Ok(result);
        }



        /// <summary>
        /// Adds a new movie
        /// </summary>
        /// <param name="movieModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin,super-user")]
        [HttpPost]
        public async Task<ActionResult<ResponseModel<MovieDomainModel>>> Post([FromBody] MovieModel movieModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MovieDomainModel domainModel = new MovieDomainModel
            {
                Current = movieModel.Current,
                Rating = movieModel.Rating,
                Title = movieModel.Title,
                Year = movieModel.Year,
                HasOscar = movieModel.hasOscar,
                Tags = new List<TagDomainModel>(),
                BannerUrl = movieModel.BannerUrl,
                TrailerUrl = movieModel.TrailerUrl
            };

            foreach(var item in movieModel.Tags)
            {
                domainModel.Tags.Add(new TagDomainModel
                {
                    TagName = item
                });
            }

            ResponseModel<MovieDomainModel> createMovie;

            try
            {
                createMovie = await _movieService.AddMovie(domainModel);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (createMovie == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_CREATION_ERROR,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            if (createMovie.IsSuccessful == false)
            {
                return BadRequest(createMovie.ErrorMessage);
            }

            return Created("movies//" + createMovie.DomainModel.Id, createMovie.DomainModel);
        }

        /// <summary>
        /// Updates a movie
        /// </summary>
        /// <param name="id"></param>
        /// <param name="movieModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin,super-user")]
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<ResponseModel<MovieDomainModel>>> Put(Guid id, [FromBody] MovieModel movieModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ResponseModel<MovieDomainModel> movieToUpdate;

            movieToUpdate = await _movieService.GetMovieByIdAsync(id);

            if (movieToUpdate == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            movieToUpdate.DomainModel.Title = movieModel.Title;
            movieToUpdate.DomainModel.Current = movieModel.Current;
            movieToUpdate.DomainModel.Year = movieModel.Year;
            movieToUpdate.DomainModel.Rating = movieModel.Rating;

            ResponseModel<MovieDomainModel> movieDomainModel;
            try
            {
                movieDomainModel = await _movieService.UpdateMovie(movieToUpdate.DomainModel);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (movieDomainModel.IsSuccessful == false)
            {
                return BadRequest(movieDomainModel.ErrorMessage);
            }

            return Accepted("movies//" + movieDomainModel.DomainModel.Id, movieDomainModel.DomainModel);

        }

        /// <summary>
        /// Delete a movie by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin,super-user")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<ResponseModel<MovieDomainModel>>> Delete(Guid id)
        {
            ResponseModel<MovieDomainModel> deletedMovie;
            try
            {
                deletedMovie = await _movieService.DeleteMovie(id);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (deletedMovie == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            if (deletedMovie.IsSuccessful == false)
            {
                ErrorResponseModel errorResponseModel = new ErrorResponseModel
                {
                    ErrorMessage = "Movie has projections in future!",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return BadRequest(errorResponseModel);
            }

            return Accepted("movies//" + deletedMovie.DomainModel.Id, deletedMovie.DomainModel);
        }

        [Authorize(Roles = "admin,super-user")]
        [HttpPatch]
        [Route("{id}")]
        public async Task<ActionResult<ResponseModel<MovieDomainModel>>> ActivateDeactivateMovie(Guid id)
        {
            ResponseModel<MovieDomainModel> changedCurrentMovie = new ResponseModel<MovieDomainModel>();
            try
            {
                changedCurrentMovie = await _movieService.ActivateDeactivateMovie(id);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (changedCurrentMovie == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.MOVIE_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            if (changedCurrentMovie.IsSuccessful == false)
            {
                return BadRequest(changedCurrentMovie.ErrorMessage);
            }

            return Accepted("movies//" + changedCurrentMovie.DomainModel.Id, changedCurrentMovie.DomainModel);
        }

        [HttpGet]
        [Route("bytag/{tagName}")]
        public async Task<ActionResult<IEnumerable<MovieDomainModel>>> GetByTagName(string tagName)
        {
            var result = await _movieService.GetByTag(tagName);
            if(result == null)
            {
                result = new List<MovieDomainModel>();
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("bytitle/{title}")]
        public async Task<ActionResult<IEnumerable<MovieDomainModel>>> GetByTitle(string title)
        {
            var result = await _movieService.GetByTitle(title);
            if(result == null)
            {
                result = new List<MovieDomainModel>();
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("byyear/{year}")]
        public async Task<ActionResult<IEnumerable<MovieDomainModel>>> GetByYear(string year)
        {
            var result = await _movieService.GetByYear(year);
            if (result == null)
            {
                result = new List<MovieDomainModel>();
            }
            return Ok(result);
        }
    }
}
