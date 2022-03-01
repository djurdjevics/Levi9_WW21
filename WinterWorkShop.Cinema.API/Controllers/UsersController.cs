using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDomainModel>>> GetAsync()
        {
            IEnumerable<UserDomainModel> userDomainModels;

            userDomainModels = await _userService.GetAllAsync();

            if (userDomainModels == null)
            {
                userDomainModels = new List<UserDomainModel>();
            }

            return Ok(userDomainModels);
        }

        /// <summary>
        /// Gets User by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<ActionResult<UserDomainModel>> GetbyIdAsync(Guid id)
        {
            UserDomainModel model;

            model = await _userService.GetUserByIdAsync(id);

            if (model == null)
            {
                return NotFound(Messages.USER_NOT_FOUND);
            }

            return Ok(model);
        }

        // <summary>
        /// Gets User by UserName
        /// </summary>
        /// <param name="projectionModel"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("username/{username}/")]
        public async Task<ActionResult<UserDomainModel>> GetbyUserNameAsync(string username)
        {
            UserDomainModel model;

            model = await _userService.GetUserByUserName(username);

            if (model == null)
            {
                return NotFound(new ErrorResponseModel() { StatusCode = System.Net.HttpStatusCode.NotFound, ErrorMessage = Messages.USER_NOT_FOUND });
            }

            return Ok(model);
        }

        // <summary>
        /// Create new User
        /// </summary>
        /// <param name="createUserModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserModel userModel)
        {
            UserDomainModel domainModel = new UserDomainModel()
            {
                FirstName = userModel.firstName,
                LastName = userModel.lastName,
                UserName = userModel.userName,
                BonusPoints = 0,
                Role = userModel.role
            };

            ResponseModel<UserDomainModel> createUser;

            try
            {
                createUser = await _userService.AddUser(domainModel);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorresponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorresponse);
            }

            if (!createUser.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = createUser.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Created("users//" + createUser.DomainModel.Id, createUser.DomainModel);
        }


    }
}
