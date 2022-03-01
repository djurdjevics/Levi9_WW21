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
    public interface IAuditoriumsRepository : IRepository<Auditorium> 
    {
        Task<IEnumerable<Auditorium>> GetByAuditName(string name, int id);
        Task<IEnumerable<Auditorium>> GetByCinemaId(int cinemaId);
        Task<IEnumerable<Auditorium>> DeleteByCinemaId(int cinemaId);
    }
    public class AuditoriumsRepository : IAuditoriumsRepository
    {
        private CinemaContext _cinemaContext;

        public AuditoriumsRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }


        public async Task<IEnumerable<Auditorium>> GetByAuditName(string name, int id)
        {
            var data = await _cinemaContext.Auditoriums.Where(x => x.Name.Equals(name) && x.CinemaId.Equals(id)).ToListAsync();
            return data;
        }

        public Auditorium Delete(object id)
        {
            Auditorium existing = _cinemaContext.Auditoriums.Find(id);
            var result = _cinemaContext.Auditoriums.Remove(existing);

            return result.Entity;
        }

        public async Task<IEnumerable<Auditorium>> GetAll()
        {
            var data = await _cinemaContext.Auditoriums.Include(x=>x.Seats).ToListAsync();

            return data;
        }

        public async Task<Auditorium> GetByIdAsync(object id)
        {
            return await _cinemaContext.Auditoriums.FindAsync(id);
        }

        public Auditorium Insert(Auditorium obj)
        {
            var data = _cinemaContext.Auditoriums.Add(obj).Entity;

            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Auditorium Update(Auditorium obj)
        {
            var updatedEntry = _cinemaContext.Auditoriums.Attach(obj);
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry.Entity;
        }

        public async Task<IEnumerable<Auditorium>> GetByCinemaId(int cinemaId)
        {
            var result = _cinemaContext.Auditoriums.Where(x => x.CinemaId == cinemaId);
            return result;
        }

        public async Task<IEnumerable<Auditorium>> DeleteByCinemaId(int cinemaId)
        {
            IEnumerable<Auditorium> existing = _cinemaContext.Auditoriums.Where(x => x.CinemaId == cinemaId);
            List<Auditorium> result = new List<Auditorium>();
            foreach (Auditorium auditorium in existing)
            {
                var data = _cinemaContext.Remove(auditorium);
                result.Add(data.Entity);
            }

            return result;
        }
    }
}
