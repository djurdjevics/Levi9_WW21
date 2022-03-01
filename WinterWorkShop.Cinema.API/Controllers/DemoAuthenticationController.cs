using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.TokenServiceExtensions;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [OpenApiIgnore]
    public class DemoAuthenticationController : ControllerBase    
    {
        private readonly IConfiguration _configuration;

        public DemoAuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // NOT FOR PRODUCTION USE!!!
        // you will need a robust auth implementation for production
        // i.e. try IdentityServer4
        [Route("/get-token")]
        public IActionResult GenerateToken(string userName = "", string name = "aspnetcore-workshop-demo", string role = "admin")
        {
            var jwt = JwtTokenGenerator
                .Generate(userName, name, role, _configuration["Tokens:Issuer"], _configuration["Tokens:Key"]);

            return Ok(new {token = jwt});
        }
    }
}
