using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IMovieWithTagRepository : IRepository<MovieWithTag>
    {
        public Task<IEnumerable<MovieWithTag>> GetByMovieID(object movieId);
        public Task<IEnumerable<MovieWithTag>> GetByTagName(object tagName);
    }
    public class MovieWithTagRepository : IMovieWithTagRepository
    {
        private CinemaContext _cinemaContext;
        public MovieWithTagRepository(CinemaContext ctx)
        {
            _cinemaContext = ctx;
        }
        public MovieWithTag Delete(object id)
        {
            var objectToDelete = _cinemaContext.MovieWithTag.Find(id);
            var result = _cinemaContext.Remove(objectToDelete).Entity;
            return result;
        }

        public async Task<IEnumerable<MovieWithTag>> GetAll()
        {
            var result = await _cinemaContext.MovieWithTag.Include(x => x.Movie).Include(x => x.Tag).ToListAsync();
            return result;
        }

        public async Task<MovieWithTag> GetByIdAsync(object id)
        {
            var result = await _cinemaContext.MovieWithTag.Include(x => x.Movie).Include(x => x.Tag).SingleOrDefaultAsync(x => x.TagId == (Guid)id);
            return result;
        }

        public async Task<IEnumerable<MovieWithTag>> GetByMovieID(object MovieId)
        {
            var result = await _cinemaContext.MovieWithTag.Include(x => x.Movie).Include(x => x.Tag).Where(x => x.MovieId == (Guid)MovieId).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<MovieWithTag>> GetByTagName(object tagName)
        {
            var result = await _cinemaContext.MovieWithTag.Include(x => x.Movie).Include(x => x.Tag).Where(x =>  x.Tag.TagName.ToLower() == ((string)tagName).ToLower()).ToListAsync();
            return result;
        }

        public MovieWithTag Insert(MovieWithTag obj)
        {
            var result = _cinemaContext.MovieWithTag.Add(obj).Entity;
            return result;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public MovieWithTag Update(MovieWithTag obj)
        {
            var result = _cinemaContext.Attach(obj).Entity;
            _cinemaContext.Entry(obj).State = EntityState.Modified;
            return result;
        }
    }
}
