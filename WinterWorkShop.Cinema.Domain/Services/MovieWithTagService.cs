using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class MovieWithTagService:IMovieWithTagService
    {
        private IMovieWithTagRepository _movieWithTagRepo;
        public MovieWithTagService(IMovieWithTagRepository movieWithTagRepository)
        {
            _movieWithTagRepo = movieWithTagRepository;
        }

        public async Task<IEnumerable<MovieWithTagDomainModel>> GetAll()
        {
            var data = await _movieWithTagRepo.GetAll();
            List<MovieWithTagDomainModel> result = new List<MovieWithTagDomainModel>();
            foreach(var item in data)
            {
                result.Add(new MovieWithTagDomainModel
                {
                    MovieId = item.MovieId,
                    TagId = item.TagId,
                    MovieTitle = item.Movie.Title,
                    TagName = item.Tag.TagName
                });
            }
            return result;
        }

        public async Task<IEnumerable<MovieWithTagDomainModel>> GetByMovieId(object id)
        {
            var data = await _movieWithTagRepo.GetByMovieID(id);
            List<MovieWithTagDomainModel> result = new List<MovieWithTagDomainModel>();
            foreach(var item in data)
            {
                result.Add(new MovieWithTagDomainModel
                {
                    MovieId = item.MovieId,
                    TagId = item.TagId,
                    MovieTitle = item.Movie.Title,
                    TagName = item.Tag.TagName
                }) ;
            }
            return result;
        }

        //public async Task<IEnumerable<MovieDomainModel>> GetByTagName(object tagName)
        //{
        //    var data = await _movieWithTagRepo.GetByTagName(tagName);
        //    List<MovieDomainModel> result = new List<MovieDomainModel>();
        //    foreach(var item in data)
        //    {
        //        result.Add(new MovieDomainModel
        //        {
        //            Id = item.Movie.Id,
        //            Current = item.Movie.Current,
        //            HasOscar = item.Movie.HasOscar,
        //            Rating = item.Movie.Rating??0,
        //            Title = item.Movie.Title,
        //            Year = item.Movie.Year
        //        });
        //    }
        //    return result;
        //}

        public async Task<ResponseModel<MovieWithTagDomainModel>> Create(MovieWithTagDomainModel obj)
        {
            MovieWithTag movieWithTag = new MovieWithTag
            {
                MovieId = obj.MovieId,
                TagId = obj.TagId
            };
            var data = _movieWithTagRepo.Insert(movieWithTag);
            if(data == null)
            {
                return new ResponseModel<MovieWithTagDomainModel>
                {
                    IsSuccessful = false,
                    ErrorMessage = "Failed to assign tags!"
                };
            }

            MovieWithTagDomainModel result = new MovieWithTagDomainModel
            {
                MovieId = data.MovieId,
                TagId = data.TagId,
            };

            return new ResponseModel<MovieWithTagDomainModel>
            {
                IsSuccessful = true,
                DomainModel = result
            };
        }
    }
}
