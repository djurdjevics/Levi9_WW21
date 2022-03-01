using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface IAuditoriumService
    {
        Task<IEnumerable<AuditoriumDomainModel>> GetAllAsync();
        Task<AuditoriumDomainModel> GetAuditoriumByIdAsync(int id);
        Task<IEnumerable<AuditoriumDomainModel>> GetAuditoriumByCinemaId(int cinemaId);
        Task<AuditoriumDomainModel> DeleteAuditorium(int id);
        Task<IEnumerable<AuditoriumDomainModel>> DeleteAuditoriumByCinemaId(int cinemaId);
        Task<CreateAuditoriumResultModel> CreateAuditorium(AuditoriumDomainModel domainModel, int numberOfRows, int numberOfSeats);
        Task<AuditoriumDomainModel> UpdateAuditorium(AuditoriumDomainModel updateAuditorium, int numberOfRows, int numberOfSeats);
    }
}
