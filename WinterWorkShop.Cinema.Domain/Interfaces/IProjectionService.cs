using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface IProjectionService
    {
        Task<IEnumerable<ProjectionDomainModel>> GetAllAsync();
        Task<CreateProjectionResultModel> CreateProjection(ProjectionDomainModel domainModel);
        Task<ResponseModel<ProjectionDomainModel>> Delete(Guid id);
        Task<ResponseModel<ProjectionDomainModel>> Update(ProjectionDomainModel projectionDomainModel);
        Task<ResponseModel<ProjectionDomainModel>> GetProjectionById(Guid id);
        Task<ResponseModel<IEnumerable<ProjectionDomainModel>>> GetProjectionsFiltered(FilterProjectionsModel filterProjectionsModel);
    }
}
