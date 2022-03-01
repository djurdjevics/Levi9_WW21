using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketsRepository _ticketsRepository;
        private readonly IProjectionService _projectionService;
        private readonly ISeatService _seatService;
        private readonly IUserService _userService;
        
        public TicketService(ITicketsRepository ticketsRepository, IProjectionService projectionService, IUserService userService, ISeatService seatService)
        {
            _ticketsRepository = ticketsRepository;
            _projectionService = projectionService;
            _seatService = seatService;
            _userService = userService;
        }

        public async Task<IEnumerable<TicketDomainModel>> GetAllTickets()
        {
            var data = await _ticketsRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<TicketDomainModel> result = new List<TicketDomainModel>();
            TicketDomainModel model;
            foreach (var item in data)
            {
                model = new TicketDomainModel
                {
                    Id = item.Id,
                    ProjectionId = item.ProjectionId,
                    SeatId = item.SeatId,
                    UserId = item.UserId
                };
                result.Add(model);
            }

            return result;

        }

        public async Task<TicketDomainModel> DeleteTicket(Guid id)
        {
            var ticket = await _ticketsRepository.GetByIdAsync(id);
            if (ticket == null)
                return null;

            var projection = await _projectionService.GetProjectionById(ticket.ProjectionId);
            if (projection == null)
                return null;

            if (projection.DomainModel.ProjectionTime.Date <= DateTime.Now.Date)
                return null;

            var data = _ticketsRepository.Delete(id);
            if (data == null)
                return null;

            var result = new TicketDomainModel()
            {
                Id = data.Id,
                Price = data.Price,
                ProjectionId = data.ProjectionId,
                SeatId = data.SeatId,
                UserId = data.UserId
            };

            return result;
        }

        public async Task<ResponseModel<TicketDomainModel>> GetTicketByIdAsync(Guid id)
        {
            var data = await _ticketsRepository.GetByIdAsync(id);

            if (data == null)
                return new ResponseModel<TicketDomainModel>()
                {
                    DomainModel = null,
                    IsSuccessful = false,
                    ErrorMessage = Messages.TICKET_NOT_FOUND
                };

            var result = new ResponseModel<TicketDomainModel>()
            {
                DomainModel = new TicketDomainModel()
                {
                    Id = data.Id,
                    Price = data.Price,
                    ProjectionId = data.ProjectionId,
                    SeatId = data.SeatId,
                    UserId = data.UserId
                },
                ErrorMessage = null,
                IsSuccessful = true
             }; 

            return result;
        }

        public ResponseModel<IEnumerable<TicketDomainModel>> GetTicketsByProjectionId(Guid projectionId)
        {
            var data = _ticketsRepository.GetTicketsByProjectionId(projectionId);

            if (data == null)
                return new ResponseModel<IEnumerable<TicketDomainModel>>()
                {
                    DomainModel = null,
                    IsSuccessful = false,
                    ErrorMessage = Messages.TICKET_PROJECTION_CAN_NOT_BE_FOUND
                };


            var result = new ResponseModel<IEnumerable<TicketDomainModel>>()
            {
                DomainModel = data.Select(ticket => new TicketDomainModel
                {
                    Id = ticket.Id,
                    Price = ticket.Price,
                    ProjectionId = ticket.ProjectionId,
                    SeatId = ticket.SeatId,
                    UserId = ticket.UserId
                }).ToList(),
                ErrorMessage = null,
                IsSuccessful = true
            };

            return result;
        }
        public async Task<ResponseModel<IEnumerable<SeatDomainModel>>> GetBusySeats(Guid projectionId)
        {
            var ticketsForProjection = _ticketsRepository.GetTicketsByProjectionId(projectionId);
            if (ticketsForProjection == null)
                return new ResponseModel<IEnumerable<SeatDomainModel>>()
                {
                    DomainModel = null,
                    IsSuccessful = false,
                    ErrorMessage = Messages.TICKET_PROJECTION_CAN_NOT_BE_FOUND
                };

            var seats = new List<Seat>();
            foreach (var item in ticketsForProjection)
            {
                ResponseModel<SeatDomainModel> seat = null;
                seat = await _seatService.GetSeatByIdAsync(item.SeatId);

                if (!seat.IsSuccessful)
                    return new ResponseModel<IEnumerable<SeatDomainModel>>()
                    {
                        DomainModel = null,
                        IsSuccessful = false,
                        ErrorMessage = seat.ErrorMessage
                    };

                var seatModel = new Seat()
                {
                    AuditoriumId = seat.DomainModel.AuditoriumId,
                    Id = seat.DomainModel.Id,
                    Number = seat.DomainModel.Number,
                    Row = seat.DomainModel.Row
                };
                seats.Add(seatModel);
            }

            var result = new ResponseModel<IEnumerable<SeatDomainModel>>()
            {
                DomainModel = seats.Select(seat => new SeatDomainModel
                {
                    AuditoriumId = seat.AuditoriumId,
                    Id = seat.Id,
                    Number = seat.Number,
                    Row = seat.Row
                }).ToList(),
                ErrorMessage = null,
                IsSuccessful = true
            };

            return result; 
        }
        public async Task<ResponseModel<TicketDomainModel>> AddTicket(Guid seatId, Guid projectionId, double price, string userName)
        {
            var seatIdExists = await _seatService.GetSeatByIdAsync(seatId);
            if (!seatIdExists.IsSuccessful)
                return new ResponseModel<TicketDomainModel>()
                {
                    DomainModel = null,
                    ErrorMessage = seatIdExists.ErrorMessage,
                    IsSuccessful = false
                };

            var projectionIdExists = await _projectionService.GetProjectionById(projectionId);
            if(!projectionIdExists.IsSuccessful)
                return new ResponseModel<TicketDomainModel>()
                {
                    DomainModel = null,
                    ErrorMessage = projectionIdExists.ErrorMessage,
                    IsSuccessful = false
                };

            if(seatIdExists.DomainModel.AuditoriumId != projectionIdExists.DomainModel.AuditoriumId)
                return new ResponseModel<TicketDomainModel>()
                {
                    DomainModel = null,
                    ErrorMessage = Messages.TICKET_CREATION_ERROR_AUDITORIUM_ID_DOESNT_MATCH,
                    IsSuccessful = false
                };


            var ticketsForProjection = _ticketsRepository.GetTicketsByProjectionId(projectionId);
            if (ticketsForProjection == null)
                return new ResponseModel<TicketDomainModel>()
                {
                    DomainModel = null,
                    ErrorMessage = Messages.TICKET_PROJECTION_CAN_NOT_BE_FOUND,
                    IsSuccessful = false
                };

            var seatExist = ticketsForProjection.SingleOrDefault(x => x.SeatId == seatId);
            if (seatExist != null)
                return new ResponseModel<TicketDomainModel>()
                {
                    DomainModel = null,
                    ErrorMessage = Messages.TICKET_SEAT_ALREADY_TAKEN,
                    IsSuccessful = false
                };

            var user = await _userService.GetUserByUserName(userName);
            if (user == null)
                return new ResponseModel<TicketDomainModel>()
                {
                    DomainModel = null,
                    ErrorMessage = Messages.USER_NOT_FOUND,
                    IsSuccessful = false
                };

            var ticket = new Ticket()
            {
                Price = price,
                ProjectionId = projectionId,
                SeatId = seatId,
                UserId = user.Id
            };

            var insert = _ticketsRepository.Insert(ticket);
            if (insert == null)
                return new ResponseModel<TicketDomainModel>
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.TICKET_CREATION_ERROR
                };
            _ticketsRepository.Save();

            var response = new ResponseModel<TicketDomainModel>()
            {
                DomainModel = new TicketDomainModel()
                {
                    Id = insert.Id,
                    Price = insert.Price,
                    ProjectionId = insert.ProjectionId,
                    SeatId = insert.SeatId,
                    UserId = insert.UserId
                },
                ErrorMessage = null,
                IsSuccessful = true
            };
            return response;
        }

    }
}
