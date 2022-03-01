using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MovieWithTagsController : ControllerBase
    {
        private IMovieWithTagService _movieWithTagService;
        public MovieWithTagsController(IMovieWithTagService movieWithTagService)
        {
            _movieWithTagService = movieWithTagService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieWithTagDomainModel>>> GetAll()
        {
            var data = await _movieWithTagService.GetAll();
            if (data == null)
            {
                data = new List<MovieWithTagDomainModel>();
            }
            return Ok(data);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<IEnumerable<MovieWithTagDomainModel>>> GetByMovieId(Guid id)
        {
            var data = await _movieWithTagService.GetByMovieId(id);
            if(data == null)
            {
                data =  new List<MovieWithTagDomainModel>();
            }
            return Ok(data);
        }
    }
}
