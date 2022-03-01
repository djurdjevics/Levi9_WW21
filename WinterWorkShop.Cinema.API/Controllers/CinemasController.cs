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
    public class CinemasController : ControllerBase
    {
        private readonly ICinemaService _cinemaService;

        public CinemasController(ICinemaService cinemaService)
        {
            _cinemaService = cinemaService;
        }

        /// <summary>
        /// Gets all cinemas
        /// </summary>
        /// <returns>List of cinemas</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CinemaDomainModel>>> GetAsync()
        {
            IEnumerable<CinemaDomainModel> cinemaDomainModels;

            cinemaDomainModels = await _cinemaService.GetAllAsync();

            if (cinemaDomainModels == null)
            {
                cinemaDomainModels = new List<CinemaDomainModel>();
            }

            return Ok(cinemaDomainModels);
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CinemaDomainModel>> GetByIdAsync(int id)
        {
            CinemaDomainModel cinema;

            cinema = await _cinemaService.GetCinemaByIdAsync(id);

            if (cinema == null)
            {
                return NotFound(Messages.CINEMA_DOES_NOT_EXIST);
            }

            return Ok(cinema);
        }

        [HttpDelete]
        [Route("{cinemaId}")]
        public async Task<ActionResult> Delete(int cinemaId)
        {
            CinemaDomainModel deletedCinema;

            try
            {
                deletedCinema = await _cinemaService.DeleteCinema(cinemaId);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.CINEMA_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest

                };
                return BadRequest(errorResponse);
            }
            catch (ArgumentNullException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.CINEMA_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (deletedCinema == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.CINEMA_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Accepted("Cinemas//" + deletedCinema.Id, deletedCinema);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CinemaWithAuditoriumModel cinemaModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CinemaDomainModel cinema = new CinemaDomainModel()
            {
                Name = cinemaModel.Name
            };

            CreateCinemaWithAuditoriumModel cinemaWithAuditorium = new CreateCinemaWithAuditoriumModel
            {
                AuditoriumName = cinemaModel.auditName,
                CinemaName = cinemaModel.Name,
                NumberOfRows = cinemaModel.seatRows,
                NumberOfColumns = cinemaModel.numberOfSeats
            };

            CreateCinemaResultModel createCinema;
            try
            {
                if (cinemaModel.auditName != "" && cinemaModel.seatRows > 0 && cinemaModel.numberOfSeats > 0)
                {
                    createCinema = await _cinemaService.AddCinemaWithAuditorium(cinemaWithAuditorium);
                }
                else
                {
                    createCinema = await _cinemaService.AddCinema(cinema);
                }
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (createCinema.IsSuccessful != true)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel()
                {
                    ErrorMessage = createCinema.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            return Created("Cinemas//" + createCinema.Cinema.Id, createCinema.Cinema);
        }


        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CreateCinemaModel cinemaModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CinemaDomainModel updateCinema;

            updateCinema = await _cinemaService.GetCinemaByIdAsync(id);

            if (updateCinema == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.CINEMA_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            updateCinema.Name = cinemaModel.Name;

            CinemaDomainModel cinemaDomainModel;
            try
            {
                cinemaDomainModel = await _cinemaService.UpdateCinema(updateCinema);
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

            return Accepted("Cinemas//" + cinemaDomainModel.Id, cinemaDomainModel);

        }
    }
}
