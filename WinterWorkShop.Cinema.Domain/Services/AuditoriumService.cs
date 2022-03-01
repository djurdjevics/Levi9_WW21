using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class AuditoriumService : IAuditoriumService
    {
        private readonly IAuditoriumsRepository _auditoriumsRepository;
        private readonly ICinemasRepository _cinemasRepository;
        private readonly ISeatService _seatService;

        public AuditoriumService(IAuditoriumsRepository auditoriumsRepository, ICinemasRepository cinemasRepository, ISeatService seatService)
        {
            _auditoriumsRepository = auditoriumsRepository;
            _cinemasRepository = cinemasRepository;
            _seatService = seatService;
        }

        public async Task<IEnumerable<AuditoriumDomainModel>> GetAllAsync()
        {
            var data = await _auditoriumsRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<AuditoriumDomainModel> result = new List<AuditoriumDomainModel>();
            AuditoriumDomainModel model;
            foreach (var item in data)
            {
                model = new AuditoriumDomainModel
                {
                    Id = item.Id,
                    CinemaId = item.CinemaId,
                    Name = item.Name
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<AuditoriumDomainModel> GetAuditoriumByIdAsync(int id)
        {
            var data = await _auditoriumsRepository.GetByIdAsync(id);
            if (data == null)
            {
                return null;
            }

            List<SeatDomainModel> domainModelList = new List<SeatDomainModel>();
            if (data.Seats != null)
            {
                foreach (Seat seat in data.Seats)
                {

                    SeatDomainModel domainModel = new SeatDomainModel()
                    {
                        Id = seat.Id,
                        AuditoriumId = seat.AuditoriumId,
                        Number = seat.Number,
                        Row = seat.Row
                    };
                    domainModelList.Add(domainModel);
                }
            }

            AuditoriumDomainModel result = new AuditoriumDomainModel()
            {
                Id = data.Id,
                CinemaId = data.CinemaId,
                Name = data.Name,
                SeatsList = domainModelList
            };

            return result;
        }

        public async Task<IEnumerable<AuditoriumDomainModel>> GetAuditoriumByCinemaId(int cinemaId)
        {
            var data = await _auditoriumsRepository.GetByCinemaId(cinemaId);

            if (data == null)
            {
                return null;
            }

            List<AuditoriumDomainModel> result = new List<AuditoriumDomainModel>();
            AuditoriumDomainModel model;
            foreach (var item in data)
            {
                model = new AuditoriumDomainModel
                {
                    Id = item.Id,
                    CinemaId = item.CinemaId,
                    Name = item.Name
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<AuditoriumDomainModel> DeleteAuditorium(int id)
        {
            var deletedSeats = await _seatService.DeleteByAuditoriumId(id);
            if (deletedSeats == null)
            {
                return null;
            }

            var deletedAuditorium = _auditoriumsRepository.Delete(id);
            if (deletedAuditorium == null)
            {
                return null;
            }

            _auditoriumsRepository.Save();

            AuditoriumDomainModel result = new AuditoriumDomainModel
            {
                CinemaId = deletedAuditorium.CinemaId,
                Id = deletedAuditorium.Id,
                Name = deletedAuditorium.Name,
                SeatsList = deletedSeats.ToList()
            };

            return result;
        }

        public async Task<IEnumerable<AuditoriumDomainModel>> DeleteAuditoriumByCinemaId(int cinemaId)
        {
            var auditoriums = await _auditoriumsRepository.GetByCinemaId(cinemaId);

            if (auditoriums == null)
            {
                return null;
            }

            List<Auditorium> auditoriumList = auditoriums.ToList();

            List<AuditoriumDomainModel> deletedAuditoriums = new List<AuditoriumDomainModel>();

            foreach (Auditorium auditorium in auditoriumList)
            {
                var deletedSeats = await _seatService.DeleteByAuditoriumId(auditorium.Id);
                if (deletedSeats == null)
                {
                    return null;
                }

                var deletedAuditorium = _auditoriumsRepository.Delete(auditorium.Id);
                if (deletedAuditorium == null)
                {
                    return null;
                }

                AuditoriumDomainModel domainModel = new AuditoriumDomainModel
                {
                    CinemaId = deletedAuditorium.CinemaId,
                    Id = deletedAuditorium.Id,
                    Name = deletedAuditorium.Name
                };

                deletedAuditoriums.Add(domainModel);
            }

            return deletedAuditoriums;
        }

        public async Task<CreateAuditoriumResultModel> CreateAuditorium(AuditoriumDomainModel domainModel, int numberOfRows, int numberOfSeats)
        {
            var cinema = await _auditoriumsRepository.GetByCinemaId(domainModel.CinemaId);
            //var cinemaSameName = cinema.ToList();
            //if (cinemaSameName != null && cinemaSameName.Count > 0)
            //{
            //    return new CreateAuditoriumResultModel
            //    {
            //        IsSuccessful = false,
            //        ErrorMessage = Messages.AUDITORIUM_UNVALID_CINEMAID
            //    };
            //}

            var auditorium = await _auditoriumsRepository.GetByAuditName(domainModel.Name, domainModel.CinemaId);
            var sameAuditoriumName = auditorium.ToList();
            if (sameAuditoriumName != null && sameAuditoriumName.Count > 0)
            {
                return new CreateAuditoriumResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.AUDITORIUM_SAME_NAME
                };
            }

            Auditorium newAuditorium = new Auditorium
            {
                Name = domainModel.Name,
                CinemaId = domainModel.CinemaId,
            };

            newAuditorium.Seats = new List<Seat>();

            for (int i = 1; i <= numberOfRows; i++)
            {
                for (int j = 1; j <= numberOfSeats; j++)
                {
                    Seat newSeat = new Seat()
                    {
                        Row = i,
                        Number = j
                    };

                    newAuditorium.Seats.Add(newSeat);
                }
            }

            Auditorium insertedAuditorium = _auditoriumsRepository.Insert(newAuditorium);
            if (insertedAuditorium == null)
            {
                return new CreateAuditoriumResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.AUDITORIUM_CREATION_ERROR
                };
            }

            _auditoriumsRepository.Save();
            CreateAuditoriumResultModel resultModel = new CreateAuditoriumResultModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Auditorium = new AuditoriumDomainModel
                {
                    Id = insertedAuditorium.Id,
                    Name = insertedAuditorium.Name,
                    CinemaId = insertedAuditorium.CinemaId,
                    SeatsList = new List<SeatDomainModel>()
                }
            };

            foreach (var item in insertedAuditorium.Seats)
            {
                resultModel.Auditorium.SeatsList.Add(new SeatDomainModel
                {
                    AuditoriumId = insertedAuditorium.Id,
                    Id = item.Id,
                    Number = item.Number,
                    Row = item.Row
                });
            }
            _auditoriumsRepository.Save();

            return resultModel;
        }

        public async Task<AuditoriumDomainModel> UpdateAuditorium(AuditoriumDomainModel updateAuditorium, int numberOfRows, int numberOfSeats)
        {
            Data.Auditorium auditorium = new Data.Auditorium()
            {
                Id = updateAuditorium.Id,
                CinemaId = updateAuditorium.CinemaId,
                Name = updateAuditorium.Name
            };

            auditorium.Seats = new List<Seat>();

            for (int i = 1; i <= numberOfRows; i++)
            {
                for (int j = 1; j <= numberOfSeats; j++)
                {
                    Seat updateSeat = new Seat()
                    {
                        Row = i,
                        Number = j
                    };

                    auditorium.Seats.Add(updateSeat);
                }

            }
            var data = _auditoriumsRepository.Update(auditorium);

            if (data == null)
            {
                return null;
            }

            _auditoriumsRepository.Save();



            AuditoriumDomainModel domailModel = new AuditoriumDomainModel
            {
                Id = data.Id,
                Name = data.Name,
                CinemaId = data.CinemaId,
                SeatsList = new List<SeatDomainModel>()
            };
           

            foreach (var item in data.Seats)
            {
                domailModel.SeatsList.Add(new SeatDomainModel
                {
                    AuditoriumId = updateAuditorium.Id,
                    Id = item.Id,
                    Number = item.Number,
                    Row = item.Row
                });
            }
            _auditoriumsRepository.Save();

            return domailModel;
        
        }
    }
}
