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
    public interface ISeatsRepository : IRepository<Seat>
    {
        IEnumerable<Seat> GetByAuditoriumId(int auditoriumId);
        Task<IEnumerable<Seat>> DeleteByAuditoriumId(int auditoriumId);
    }
    public class SeatsRepository : ISeatsRepository
    {
        private CinemaContext _cinemaContext;

        public SeatsRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public Seat Delete(object id)
        {
            Seat existing = _cinemaContext.Seats.Find(id);
            if (existing == null)
                return null;

            var result = _cinemaContext.Seats.Remove(existing).Entity;

            return result;
        }

        public async Task<IEnumerable<Seat>> GetAll()
        {
            var data = await _cinemaContext.Seats.ToListAsync();

            return data;
        }

        public async Task<Seat> GetByIdAsync(object id)
        {
            return await _cinemaContext.Seats.FindAsync(id);
        }

        public Seat Insert(Seat obj)
        {
            var data = _cinemaContext.Seats.Add(obj).Entity;

            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Seat Update(Seat obj)
        {
            var updatedEntry = _cinemaContext.Seats.Attach(obj).Entity;
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry;
        }

        public IEnumerable<Seat> GetByAuditoriumId(int auditoriumId)
        {
            var projectionsData = _cinemaContext.Seats.Where(x => x.AuditoriumId == auditoriumId);

            return projectionsData;
        }
        public async Task<IEnumerable<Seat>> DeleteByAuditoriumId(int auditoriumId)
        {
            IEnumerable<Seat> seatList = _cinemaContext.Seats.Where(x => x.AuditoriumId == auditoriumId);
            List<Seat> result = new List<Seat>();
            foreach (Seat seat in seatList)
            {
                var data = _cinemaContext.Remove(seat);
                result.Add(data.Entity);
            }

            return result;
        }
    }
}
