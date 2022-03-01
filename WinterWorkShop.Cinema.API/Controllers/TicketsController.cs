using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;

        public TicketsController(ITicketService ticketService, IUserService userService)
        {
            _ticketService = ticketService;
            _userService = userService;
        }
        
        [HttpGet]
        [Route("get/all")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<TicketDomainModel>>> GetAllTickets()
        {
            IEnumerable<TicketDomainModel> ticketDomainModels;

            ticketDomainModels = await _ticketService.GetAllTickets();

            if (ticketDomainModels == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.TICKET_GET_ALL_ERROR,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }


            return Ok(ticketDomainModels);

        }
        
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<ActionResult<TicketDomainModel>> GetByIdAsync(Guid id)
        {
            ResponseModel<TicketDomainModel> responseModel = new ResponseModel<TicketDomainModel>();

            responseModel = await _ticketService.GetTicketByIdAsync(id);

            if (!responseModel.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = responseModel.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
                return NotFound(errorResponse);

            }

            return Ok(responseModel.DomainModel);
        }
        [HttpGet]
        [Route("projection/{id:guid}")]
        public async Task<ActionResult<IEnumerable<TicketDomainModel>>> GetByProjectionIdIdAsync(Guid id)
        {
            //vraca tickete a iz njih mozemo izvuci sedista
            var response =  _ticketService.GetTicketsByProjectionId(id);
            if (!response.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.TICKET_NOT_FOUND,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
                return NotFound(errorResponse);
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("busySeats/{id:guid}")]
        public async Task<ActionResult<IEnumerable<SeatDomainModel>>> GetBusySeatsByProjectionIdAsync(Guid id)
        {
            var response = await _ticketService.GetBusySeats(id);
            if (!response.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.TICKET_NOT_FOUND,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
                return NotFound(errorResponse);
            }
            return Ok(response.DomainModel);
        }

        [HttpPost]
        [Authorize(Roles = "admin, super-user, user")]
        [Route("")]
        public async Task<ActionResult<TicketDomainModel>> PostAsync([FromBody] CreateTicketModel ticketModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<TicketDomainModel> ticketResultList = new List<TicketDomainModel>();
            int bonusPointsToAdd = 0;
            foreach (var seatId in ticketModel.SeatIds)
            {
                bonusPointsToAdd++;
                TicketDomainModel domainModel = new TicketDomainModel
                {
                    Id = ticketModel.Id,
                    Price = ticketModel.Price,
                    ProjectionId = ticketModel.ProjectionId,
                    SeatId = seatId.Id,
                    UserId = ticketModel.UserId
                };
                
                string userName = User.Claims.First(c => c.Type == "UserName").Value;
                ResponseModel<TicketDomainModel> createTicketResponseModel;
                try
                {
                    createTicketResponseModel = await _ticketService.AddTicket(domainModel.SeatId, domainModel.ProjectionId, domainModel.Price, userName);
                }
                catch (DbUpdateException e)
                {
                    ErrorResponseModel errorResponse = new ErrorResponseModel
                    {
                        ErrorMessage = e.InnerException.Message ?? e.Message,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };

                    return BadRequest(errorResponse);
                }

                if (!createTicketResponseModel.IsSuccessful)
                {
                    ErrorResponseModel errorResponse = new ErrorResponseModel
                    {
                        ErrorMessage = createTicketResponseModel.ErrorMessage,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };

                    return BadRequest(errorResponse);
                }
                ticketResultList.Add(createTicketResponseModel.DomainModel);

            }
            try
            {
                var bonusPointsResult = _userService.AddBonusPoints(ticketModel.UserId, bonusPointsToAdd);

                if (bonusPointsResult == -1 || bonusPointsToAdd < 0)
                {
                    ErrorResponseModel errorResponse = new ErrorResponseModel
                    {
                        ErrorMessage = "An error occured while assigning bonus points to the User",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };

                    return BadRequest(errorResponse);
                }
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = "An error occured while assigning bonus points to the User",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }
            return Created("tickets//", ticketResultList);
        }

        [HttpGet]
        [Route("deleteProjection/{id:guid}")]
        public async Task<ActionResult<IEnumerable<TicketDomainModel>>> DeleteProjectionIfThereIsNoTicket(Guid id)
        {
            var response = _ticketService.GetTicketsByProjectionId(id);
            if (response.DomainModel == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = response.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            return Ok(response.DomainModel.ToList());
        }


    }
}
