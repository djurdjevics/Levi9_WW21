using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<TagDomainModel>> GetAll();
        Task<ResponseModel<TagDomainModel>> GetById(object id);
        Task<ResponseModel<TagDomainModel>> Create(TagDomainModel newTag);
    }
}
