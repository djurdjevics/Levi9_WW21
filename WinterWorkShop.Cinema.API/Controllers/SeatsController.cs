using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatsController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        /// <summary>
        /// Gets all seats
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SeatDomainModel>>> GetAsync()
        {
            IEnumerable<SeatDomainModel> seatDomainModels;
            
            seatDomainModels = await _seatService.GetAllAsync();

            if (seatDomainModels == null)
            {
                seatDomainModels = new List<SeatDomainModel>();
            }

            return Ok(seatDomainModels);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<ActionResult<SeatDomainModel>> GetByIdAsync(Guid id)
        {
            ResponseModel<SeatDomainModel> responseModel = new ResponseModel<SeatDomainModel>();

            responseModel = await _seatService.GetSeatByIdAsync(id);

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
        [Route("auditorium/{id:int}")]
        public async Task<ActionResult<IEnumerable<SeatDomainModel>>> GetSeatsByAuditoriumId(int id)
        {
            var response = _seatService.GetSeatsByAuditoriumId(id);
            if (response == null)
                return NotFound(Messages.SEAT_GET_ALL_SEATS_ERROR);

            return Ok(response);
        }

    }
}
