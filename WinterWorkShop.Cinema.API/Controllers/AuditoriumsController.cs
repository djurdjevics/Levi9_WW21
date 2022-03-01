using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuditoriumsController : ControllerBase
    {
        private readonly IAuditoriumService _auditoriumService;

        public AuditoriumsController(IAuditoriumService auditoriumservice)
        {
            _auditoriumService = auditoriumservice;
        }

        /// <summary>
        /// Gets all auditoriums
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditoriumDomainModel>>> GetAsync()
        {
            IEnumerable<AuditoriumDomainModel> auditoriumDomainModels;

            auditoriumDomainModels = await _auditoriumService.GetAllAsync();

            if (auditoriumDomainModels == null)
            {
                auditoriumDomainModels = new List<AuditoriumDomainModel>();
            }

            return Ok(auditoriumDomainModels);
        }

        [HttpGet]
        [Route("bycinemaid/{id}")]
        public async Task<ActionResult<IEnumerable<AuditoriumDomainModel>>> GetByCinemaId(int id)
        {
            IEnumerable<AuditoriumDomainModel> auditoriumDomainModels;

            auditoriumDomainModels = await _auditoriumService.GetAuditoriumByCinemaId(id);

            if (auditoriumDomainModels == null)
            {
                auditoriumDomainModels = new List<AuditoriumDomainModel>();
            }

            return Ok(auditoriumDomainModels);
        }

        /// <summary>
        /// Adds a new auditorium
        /// </summary>
        /// <param name="createAuditoriumModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<AuditoriumDomainModel>> PostAsync(CreateAuditoriumModel createAuditoriumModel) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AuditoriumDomainModel auditoriumDomainModel = new AuditoriumDomainModel
            {
                CinemaId = createAuditoriumModel.cinemaId,
                Name = createAuditoriumModel.auditName
            };

            CreateAuditoriumResultModel createAuditoriumResultModel;

            try 
            {
                createAuditoriumResultModel = await _auditoriumService.CreateAuditorium(auditoriumDomainModel, createAuditoriumModel.seatRows, createAuditoriumModel.numberOfSeats);
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

            if (!createAuditoriumResultModel.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel()
                {
                    ErrorMessage = createAuditoriumResultModel.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }
            
            return Created("auditoriums//" + createAuditoriumResultModel.Auditorium.Id, createAuditoriumResultModel);
        }

        [HttpDelete]
        [Authorize(Roles = "admin, super-user")]
        [Route("{auditoriumId}")]
        public async Task<ActionResult> Delete(int auditoriumId)
        {
            AuditoriumDomainModel deletedAuditorium;

            try
            {
                deletedAuditorium = await _auditoriumService.DeleteAuditorium(auditoriumId);
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
            catch (ArgumentNullException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.AUDITORIUM_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (deletedAuditorium == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.AUDITORIUM_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Accepted("auditorium//" + deletedAuditorium.Id, deletedAuditorium);
        }

          
        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CreateAuditoriumModel auditoriumModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AuditoriumDomainModel updateAuditorium;

            updateAuditorium = await _auditoriumService.GetAuditoriumByIdAsync(id);

            if (updateAuditorium == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.AUDITORIUM_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            updateAuditorium.CinemaId = auditoriumModel.cinemaId;
            updateAuditorium.Name = auditoriumModel.auditName;

            AuditoriumDomainModel auditoriumDomainModel;
            try
            {
                auditoriumDomainModel = await _auditoriumService.UpdateAuditorium(updateAuditorium, auditoriumModel.seatRows,auditoriumModel.numberOfSeats);
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
            
            return Accepted("Auditoriums//" + auditoriumDomainModel.Id, auditoriumDomainModel);
            
        }
    }
}