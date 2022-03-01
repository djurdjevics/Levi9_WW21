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
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private ITagService _tagService;
        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDomainModel>>> GetAll()
        {
            var result = await _tagService.GetAll();
            if (result == null)
            {
                return new List<TagDomainModel>();
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ResponseModel<TagDomainModel>>> GetById(object id)
        {
            var result = await _tagService.GetById(id);
            if (result.IsSuccessful == false)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.DomainModel);
        }
    }
}
