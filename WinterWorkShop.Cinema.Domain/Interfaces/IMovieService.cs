using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface IMovieService
    {
        /// <summary>
        /// Get all movies by current parameter
        /// </summary>
        /// <param name="isCurrent"></param>
        /// <returns></returns>

        IEnumerable<MovieDomainModel> GetCurrentMovies(bool? isCurrent);

        Task<IEnumerable<MovieDomainModel>> GetAllMovies();

        Task<IEnumerable<MovieDomainModel>> GetTopTenMovies();

        Task<IEnumerable<MovieDomainModel>> GetByTitle(string title);
        Task<IEnumerable<MovieDomainModel>> GetByYear(string year);
        Task<IEnumerable<MovieDomainModel>> GetTopTenMoviesByYear(int year);

        Task<IEnumerable<MovieDomainModel>> GetByTag(object tagName);


        /// <summary>
        /// Get a movie by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseModel<MovieDomainModel>> GetMovieByIdAsync(Guid id);

        Task<ResponseModel<IEnumerable<MovieDomainModel>>> GetMoviesByAuditoriumId(int id);

        /// <summary>
        /// Adds new movie to DB
        /// </summary>
        /// <param name="newMovie"></param>
        /// <returns></returns>
        Task<ResponseModel<MovieDomainModel>> AddMovie(MovieDomainModel newMovie);

        /// <summary>
        /// Update a movie to DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseModel<MovieDomainModel>> UpdateMovie(MovieDomainModel updateMovie);

        /// <summary>
        /// Delete a movie by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseModel<MovieDomainModel>> DeleteMovie(Guid id);

        Task<ResponseModel<MovieDomainModel>> ActivateDeactivateMovie(Guid id);
    }
}
