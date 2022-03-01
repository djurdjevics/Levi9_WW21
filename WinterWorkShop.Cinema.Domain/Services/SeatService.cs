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
    public class SeatService : ISeatService
    {
        private readonly ISeatsRepository _seatsRepository;

        public SeatService(ISeatsRepository seatsRepository)
        {
            _seatsRepository = seatsRepository;
        }

        public async Task<IEnumerable<SeatDomainModel>> GetAllAsync()
        {
            var data = await _seatsRepository.GetAll();

            if (data == null)
                return null;

            var result = data.Select(seat => new SeatDomainModel
            {
                Id = seat.Id,
                AuditoriumId = seat.AuditoriumId,
                Number = seat.Number,
                Row = seat.Row
            }).ToList();

            return result;
        }

        public async Task<ResponseModel<SeatDomainModel>> GetSeatByIdAsync(Guid id)
        {
            var data = await _seatsRepository.GetByIdAsync(id);

            if (data == null)
                return new ResponseModel<SeatDomainModel>()
                {
                    DomainModel = null,
                    ErrorMessage = Messages.SEAT_GET_BY_ID_ERROR,
                    IsSuccessful = false
                };

            var result = new ResponseModel<SeatDomainModel>()
            {
                DomainModel = new SeatDomainModel()
                {
                    Id = data.Id,
                    AuditoriumId = data.AuditoriumId,
                    Number = data.Number,
                    Row = data.Row
                },
                ErrorMessage = null,
                IsSuccessful = true
            };

            return result;
        }

        public async Task<ResponseModel<SeatDomainModel>> AddSeat(SeatDomainModel newSeat)
        {
            Seat seatToCreate = new Seat()
            {
                AuditoriumId = newSeat.AuditoriumId,
                Number = newSeat.Number,
                Row = newSeat.Row
            };

            #region provera_da_li_vec_postoji_to_sedista_za_taj_auditorium
            var seatsExists = await _seatsRepository.GetAll();
            var seat = seatsExists.SingleOrDefault(seat => seat.AuditoriumId == seatToCreate.AuditoriumId
                                            && seat.Number == seatToCreate.Number
                                            && seat.Row == seatToCreate.Row);
            if(seat != null)
                return new ResponseModel<SeatDomainModel>
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.SEAT_WITH_SAME_NUMBER_ROW_AUDITORIUMID_ALREADY_EXISTS
                };
            #endregion

            var data = _seatsRepository.Insert(seatToCreate);
            if(data == null)
                return new ResponseModel<SeatDomainModel>
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.SEAT_CREATION_ERROR
                };

            _seatsRepository.Save();

            ResponseModel<SeatDomainModel> result = new ResponseModel<SeatDomainModel>()
            {
                IsSuccessful = true,
                ErrorMessage = null,
                DomainModel = new SeatDomainModel()
                {
                    Id = data.Id,
                    AuditoriumId = data.AuditoriumId,
                    Number = data.Number,
                    Row = data.Row
                }
            };

            return result;
        }

        public ResponseModel<SeatDomainModel> DeleteSeat(Guid id)
        {
            var seat = _seatsRepository.Delete(id);

            if(seat == null)
                return new ResponseModel<SeatDomainModel>
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.SEAT_DOESNT_EXISTS
                };

            _seatsRepository.Save();

            var result = new ResponseModel<SeatDomainModel>()
            {
                DomainModel = new SeatDomainModel()
                {
                    Id = seat.Id,
                    AuditoriumId = seat.AuditoriumId,
                    Number = seat.Number,
                    Row = seat.Row
                },
                ErrorMessage = null,
                IsSuccessful = true
            };
            return result;
        }

        public async Task<IEnumerable<SeatDomainModel>> DeleteByAuditoriumId(int auditoriumId)
        {
            var seatModelsByAuditoriumId = _seatsRepository.GetByAuditoriumId(auditoriumId);
            if (seatModelsByAuditoriumId == null)
            {
                return null;
            }
            seatModelsByAuditoriumId.ToList();

            var data = await _seatsRepository.DeleteByAuditoriumId(auditoriumId);

            if (data == null)
            {
                return null;
            }

            var domainModelList = data.Select(seat => new SeatDomainModel()
            {
                Id = seat.Id,
                AuditoriumId = seat.AuditoriumId,
                Number = seat.Number,
                Row = seat.Row
            }).ToList();
            return domainModelList;
        }

        public IEnumerable<SeatDomainModel> GetSeatsByAuditoriumId(int auditoriumId)
        {
            var seats = _seatsRepository.GetByAuditoriumId(auditoriumId);
            if (seats == null)
                return null;

            var result = seats.Select(seat => new SeatDomainModel
            {
                AuditoriumId = seat.AuditoriumId,
                Id = seat.Id,
                Number = seat.Number,
                Row = seat.Row
            }).ToList();

            return result;
        }

        
    }
}
