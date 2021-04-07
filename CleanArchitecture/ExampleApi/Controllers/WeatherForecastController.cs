using Application.Features.ExampleFeature.Commands;
using Application.Token;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ExampleApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _mediat;
        private readonly IJwtToken _jwtToken;
        private readonly ILogger<WeatherForecastController> _logger;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator mediat, IJwtToken jwtToken)
        {
            _logger = logger;
            _mediat = mediat;
            _jwtToken = jwtToken;
        }

        [HttpGet]
        public string Get()
        {
            return _jwtToken.GenerateToken();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(CreateExampleCommand request)
        {
            await _mediat.Send(request);
            return Ok("Version 1");
        }

        [HttpGet]
        [ApiVersion("2.0")]
        public IActionResult Get2()
        {
            return Ok("Version 2");
        }
    }
}
