using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class CinemaService : ICinemaService
    {
        private readonly ICinemasRepository _cinemasRepository;
        private readonly IAuditoriumService _auditoriumService;

        public CinemaService(ICinemasRepository cinemasRepository, IAuditoriumService auditoriumService)
        {
            _cinemasRepository = cinemasRepository;
            _auditoriumService = auditoriumService;
        }

        public async Task<IEnumerable<CinemaDomainModel>> GetAllAsync()
        {
            var data = await _cinemasRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<CinemaDomainModel> result = new List<CinemaDomainModel>();
            CinemaDomainModel model;
            foreach (var item in data)
            {
                model = new CinemaDomainModel
                {
                    Id = item.Id,
                    Name = item.Name
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<CinemaDomainModel> GetCinemaByIdAsync(int id)
        {
            var data = await _cinemasRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            CinemaDomainModel domainModel = new CinemaDomainModel
            {
                Id = data.Id,
                Name = data.Name
            };

            return domainModel;
        }

        public async Task<CinemaDomainModel> DeleteCinema(int cinemaId)
        {
            var deletedAuditoriums = await _auditoriumService.DeleteAuditoriumByCinemaId(cinemaId);

            if (deletedAuditoriums == null)
            {
                return null;
            }

            var deletedCinema = _cinemasRepository.Delete(cinemaId);
            if (deletedCinema == null)
            {
                return null;
            }

            _cinemasRepository.Save();

            CinemaDomainModel result = new CinemaDomainModel
            {
                Id = deletedCinema.Id,
                Name = deletedCinema.Name
            };

            return result;
        }


        public async Task<CreateCinemaResultModel> AddCinema(CinemaDomainModel newCinema)
        {
            var cinema = await _cinemasRepository.GetByCinemaName(newCinema.Name);
            if (cinema != null)
            {
                return new CreateCinemaResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.CINEMA_SAME_NAME
                };
            }

            Data.Cinema createCinema = new Data.Cinema()
            {
                Name = newCinema.Name
            };

            var data = _cinemasRepository.Insert(createCinema);
            if (data == null)
            {
                return new CreateCinemaResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.CINEMA_CREATION_ERROR
                };
            }

            _cinemasRepository.Save();

            CinemaDomainModel cinemaDModel = new CinemaDomainModel()
            {
                Id = data.Id,
                Name = data.Name
            };

            CreateCinemaResultModel cinemaResultModel = new CreateCinemaResultModel()
            { 
                ErrorMessage = null,
                IsSuccessful = true,
                Cinema = cinemaDModel
            };

            return cinemaResultModel;
        }
        
        public async Task<CreateCinemaResultModel> AddCinemaWithAuditorium(CreateCinemaWithAuditoriumModel newCinemaWithAuditorium)
        {
            var cinemaName = await _cinemasRepository.GetByCinemaName(newCinemaWithAuditorium.CinemaName);
            if (cinemaName != null)
            {
                return new CreateCinemaResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.CINEMA_SAME_NAME
                };
            }

            Data.Cinema cinemaToCreate = new Data.Cinema()
            {
                Name = newCinemaWithAuditorium.CinemaName
            };

            var cinemaData = _cinemasRepository.Insert(cinemaToCreate);
            if (cinemaData == null)
            {
                return new CreateCinemaResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.CINEMA_CREATION_ERROR
                };
            }

            _cinemasRepository.Save();

            AuditoriumDomainModel auditoriumToCreate = new AuditoriumDomainModel()
            {
                CinemaId = cinemaData.Id,
                Name = newCinemaWithAuditorium.AuditoriumName
            };

            var auditoriumData = await _auditoriumService.CreateAuditorium(auditoriumToCreate, newCinemaWithAuditorium.NumberOfRows, newCinemaWithAuditorium.NumberOfColumns);
            if (auditoriumData == null)
            {
                return new CreateCinemaResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.AUDITORIUM_CREATION_ERROR
                };
            }

            List<AuditoriumDomainModel> auditoriumDomainModels = new List<AuditoriumDomainModel>();
            auditoriumDomainModels.Add(auditoriumData.Auditorium);

            CinemaDomainModel createdCinema = new CinemaDomainModel()
            {
                Id = cinemaData.Id,
                Name = cinemaData.Name,
                AuditoriumList = auditoriumDomainModels
            };

            CreateCinemaResultModel cinemaResultModel = new CreateCinemaResultModel()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Cinema = createdCinema
            };

            return cinemaResultModel;
        }

        public async Task<CinemaDomainModel> UpdateCinema(CinemaDomainModel updateCinema)
        {
            Data.Cinema cinema = new Data.Cinema()
            {
                Id = updateCinema.Id,
                Name = updateCinema.Name
            };

            var data = _cinemasRepository.Update(cinema);

            if (data == null)
            {
                return null;
            }
            _cinemasRepository.Save();

            CinemaDomainModel domainModel = new CinemaDomainModel()
            {
                Id = data.Id,
                Name = data.Name
            };

            return domainModel;
        }
    }

}
