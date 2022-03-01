using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.API.Models.Request;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectionsController : ControllerBase
    {
        private readonly IProjectionService _projectionService;

        public ProjectionsController(IProjectionService projectionService)
        {
            _projectionService = projectionService;
        }

        [HttpPost]
        [Route("filter")]
        public async Task<ActionResult<ResponseModel<IEnumerable<ProjectionDomainModel>>>> Filter([FromBody] FilterProjectionsModel filterProjectionsModel)
        {
            var response = await _projectionService.GetProjectionsFiltered(filterProjectionsModel);
            if (!response.IsSuccessful)
                return BadRequest(new ErrorResponseModel()
                {
                    ErrorMessage = response.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            return Ok(response.DomainModel);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<ActionResult<ResponseModel<ProjectionDomainModel>>> GetById(Guid id)
        {
            var response = await _projectionService.GetProjectionById(id);
            if(!response.IsSuccessful)
                return BadRequest(new ErrorResponseModel()
                {
                    ErrorMessage = response.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            return Ok(response.DomainModel);
        }

        /// <summary>
        /// Gets all projections
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectionDomainModel>>> GetAsync()
        {
            IEnumerable<ProjectionDomainModel> projectionDomainModels;

            projectionDomainModels = await _projectionService.GetAllAsync();

            if (projectionDomainModels == null)
            {
                projectionDomainModels = new List<ProjectionDomainModel>();
            }

            return Ok(projectionDomainModels);
        }

        /// <summary>
        /// Adds a new projection
        /// </summary>
        /// <param name="projectionModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin, super-user")]
        public async Task<ActionResult<ProjectionDomainModel>> PostAsync(CreateProjectionModel projectionModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (projectionModel.ProjectionTime < DateTime.Now)
            {
                ModelState.AddModelError(nameof(projectionModel.ProjectionTime), Messages.PROJECTION_IN_PAST);
                return BadRequest(new ErrorResponseModel() {
                    ErrorMessage = Messages.PROJECTION_IN_PAST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            ProjectionDomainModel domainModel = new ProjectionDomainModel
            {
                AuditoriumId = projectionModel.AuditoriumId,
                MovieId = projectionModel.MovieId,
                ProjectionTime = projectionModel.ProjectionTime
            };

            CreateProjectionResultModel createProjectionResultModel;

            try
            {
                createProjectionResultModel = await _projectionService.CreateProjection(domainModel);
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

            if (!createProjectionResultModel.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = createProjectionResultModel.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            return Created("projections//" + createProjectionResultModel.Projection.Id, createProjectionResultModel.Projection);
        }

        [Authorize(Roles = "admin, super-user")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult<ResponseModel<ProjectionDomainModel>>> DeleteById(Guid id)
        {
            ResponseModel<ProjectionDomainModel> result = await _projectionService.Delete(id);
            if (result.IsSuccessful == false)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.DomainModel);
        }

        [Authorize(Roles = "admin, super-user")]
        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<ResponseModel<ProjectionDomainModel>>> Update(Guid id,[FromBody]ProjectionDomainModel projectionDomainModel)
        {
            if (projectionDomainModel.ProjectionTime < DateTime.Now)
            {
                ModelState.AddModelError(nameof(projectionDomainModel.ProjectionTime), Messages.PROJECTION_IN_PAST);
                return BadRequest(new ErrorResponseModel()
                {
                    ErrorMessage = Messages.PROJECTION_IN_PAST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            ResponseModel<ProjectionDomainModel> projectionToUpdate = await _projectionService.GetProjectionById(id);
            if(projectionToUpdate.IsSuccessful == false)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = projectionToUpdate.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return BadRequest(errorResponse);
            }


            ResponseModel<ProjectionDomainModel> result;

            projectionToUpdate.DomainModel.AuditoriumId = projectionDomainModel.AuditoriumId;
            projectionToUpdate.DomainModel.MovieId = projectionDomainModel.MovieId;
            projectionToUpdate.DomainModel.ProjectionTime = projectionDomainModel.ProjectionTime;

            try
            {
                result = await _projectionService.Update(projectionToUpdate.DomainModel);
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


            if (result.IsSuccessful == false)
            {
                return BadRequest(new ErrorResponseModel
                {
                    ErrorMessage = result.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }


            return Ok(result.DomainModel);

        }
    }
}