using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface ITicketService
    {
        ResponseModel<IEnumerable<TicketDomainModel>> GetTicketsByProjectionId(Guid projectionId);
        Task<ResponseModel<TicketDomainModel>> GetTicketByIdAsync(Guid id);
        Task<TicketDomainModel> DeleteTicket(Guid id);
        Task<ResponseModel<TicketDomainModel>> AddTicket(Guid seatId, Guid projectionId, double price, string userName);
        Task<ResponseModel<IEnumerable<SeatDomainModel>>> GetBusySeats(Guid projectionId);
        Task<IEnumerable<TicketDomainModel>> GetAllTickets();
    }
}
