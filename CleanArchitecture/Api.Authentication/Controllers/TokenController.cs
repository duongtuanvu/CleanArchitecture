using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Authentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly IJwtToken _jwtToken;

        public TokenController(IJwtToken jwtToken)
        {
            _jwtToken = jwtToken;
        }

        [HttpGet]
        public string Get()
        {
            return _jwtToken.GenerateToken();
        }
    }
}
