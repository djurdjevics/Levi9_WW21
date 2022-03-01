using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface ITagRepository : IRepository<Tag>
    {
        public Task<Tag> SearchByName(string name);
    }
    public class TagRepository : ITagRepository
    {
        private CinemaContext _cinemaContext;
        public TagRepository(CinemaContext ctx)
        {
            _cinemaContext = ctx;
        }
        public Tag Delete(object id)
        {
            var objectToDelete = _cinemaContext.Tag.Find(id);
            var result = _cinemaContext.Remove(objectToDelete).Entity;
            return result;
        }

        public async Task<IEnumerable<Tag>> GetAll()
        {
            var data = await _cinemaContext.Tag.ToListAsync();
            return data;
        }

        public async Task<Tag> GetByIdAsync(object id)
        {
            var data = await _cinemaContext.Tag.FindAsync(id);
            return data;
        }

        public Tag Insert(Tag obj)
        {
            var data =  _cinemaContext.Tag.Add(obj).Entity;
            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public async Task<Tag> SearchByName(string name)
        {
            var data = await _cinemaContext.Tag.SingleOrDefaultAsync(x => x.TagName.ToLower() == name.ToLower());
            return data;
        }

        public Tag Update(Tag obj)
        {
            var updatedEntry = _cinemaContext.Tag.Attach(obj);
            _cinemaContext.Entry(obj).State = EntityState.Modified;
            return updatedEntry.Entity;
        }
    }
}
