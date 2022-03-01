using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface IMovieWithTagService
    {
        public Task<IEnumerable<MovieWithTagDomainModel>> GetAll();
        public Task<IEnumerable<MovieWithTagDomainModel>> GetByMovieId(object id);
        //public Task<IEnumerable<MovieDomainModel>> GetByTagName(object tagName);
        public Task<ResponseModel<MovieWithTagDomainModel>> Create(MovieWithTagDomainModel obj);
    }
}
