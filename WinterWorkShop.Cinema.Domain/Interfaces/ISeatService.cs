using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface ISeatService
    {
        Task<IEnumerable<SeatDomainModel>> GetAllAsync();
        Task<IEnumerable<SeatDomainModel>> DeleteByAuditoriumId(int auditoriumId);
        IEnumerable<SeatDomainModel> GetSeatsByAuditoriumId(int auditoriumId);
        Task<ResponseModel<SeatDomainModel>> GetSeatByIdAsync(Guid id);
        Task<ResponseModel<SeatDomainModel>> AddSeat(SeatDomainModel newSeat);
        ResponseModel<SeatDomainModel> DeleteSeat(Guid id);

    }
}
