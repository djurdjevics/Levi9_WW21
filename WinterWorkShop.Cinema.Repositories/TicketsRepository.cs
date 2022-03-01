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
    public interface ITicketsRepository : IRepository<Ticket>
    {
        IEnumerable<Ticket> GetTicketsByProjectionId(Guid projectionId);
    }
    public class TicketsRepository : ITicketsRepository
    {
        private CinemaContext _cinemaContext;

        public TicketsRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public Ticket Delete(object id)
        {
            var existing = _cinemaContext.Tickets.Find(id);
            if (existing == null)
                return null;

            return _cinemaContext.Tickets.Remove(existing).Entity;
        }

        public async Task<IEnumerable<Ticket>> GetAll()
        {
            return await _cinemaContext.Tickets.ToListAsync();
        }

        public async Task<Ticket> GetByIdAsync(object id)
        {
            return await _cinemaContext.Tickets.FindAsync(id);
        }

        public IEnumerable<Ticket> GetTicketsByProjectionId(Guid projectionId)
        {
            var tickets = _cinemaContext.Tickets.Where(x => x.ProjectionId == projectionId).ToList();
            return tickets;
        }

        public Ticket Insert(Ticket obj)
        {
            var data = _cinemaContext.Tickets.Add(obj).Entity;

            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Ticket Update(Ticket obj)
        {
            throw new NotImplementedException();
        }
    }
}
