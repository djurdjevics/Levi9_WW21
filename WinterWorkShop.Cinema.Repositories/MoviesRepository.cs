using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IMoviesRepository : IRepository<Movie> 
    {
        IEnumerable<Movie> GetCurrentMovies();
        Task<IEnumerable<Movie>> GetTopTenMovies();

        Task<IEnumerable<Movie>> GetTopTenMoviesByYear(int year);
        Task<IEnumerable<Movie>> GetByTitle(string title);
        Task<IEnumerable<Movie>> GetByYear(string year);
    }

    public class MoviesRepository : IMoviesRepository
    {
        private CinemaContext _cinemaContext;

        public MoviesRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public Movie Delete(object id)
        {
            Movie existing = _cinemaContext.Movies.Find(id);

            if (existing == null)
            {
                return null;
            }

            var result = _cinemaContext.Movies.Remove(existing);

            return result.Entity;
        }

        public async Task<IEnumerable<Movie>> GetAll()
        {
            return await _cinemaContext.Movies.ToListAsync();
        }

        public async Task<Movie> GetByIdAsync(object id)
        {
            var data = await _cinemaContext.Movies.FindAsync(id);

            return data;
        }

        public async Task<IEnumerable<Movie>> GetByTitle(string title)
        {
            var data = await _cinemaContext.Movies.Where(x => x.Title.Contains(title)).ToListAsync();
            return data;
        }


        public async Task<IEnumerable<Movie>> GetByYear(string year)
        {
            var data = await _cinemaContext.Movies.Where(x => x.Year.ToString() == year).ToListAsync();
            return data;
        }

        public IEnumerable<Movie> GetCurrentMovies()
        {
            var data = _cinemaContext.Movies
                .Where(x => x.Current);
            return data;
        }

        public async Task<IEnumerable<Movie>> GetMoviesbyFilter(string? title, string? year)
        {
            var list = _cinemaContext.Movies
                .Where(movie => movie.Title.Contains(title) && movie.Year.ToString().Contains(year));
            return list;
        }

        public async Task<IEnumerable<Movie>> GetTopTenMovies()
        {
            var list = (from t in _cinemaContext.Movies
                       orderby t.Rating descending
                      select t).Take(10).ToList();
            return list;
        }

        public async Task<IEnumerable<Movie>> GetTopTenMoviesByYear(int year)
        {
            var list = (from t in _cinemaContext.Movies
                        where t.Year == year
                        orderby t.Rating descending, t.HasOscar descending
                        select t).Take(10).ToList();
            return list;
        }

        public Movie Insert(Movie obj)
        {
            var data = _cinemaContext.Movies.Add(obj).Entity;

            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Movie Update(Movie obj)
        {
            var updatedEntry = _cinemaContext.Movies.Attach(obj).Entity;
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry;
        }
    }
}
