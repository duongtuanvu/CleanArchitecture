using Application.Features.ExampleFeature.Commands;
using MediatR;
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
        private readonly ILogger<WeatherForecastController> _logger;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator mediat)
        {
            _logger = logger;
            _mediat = mediat;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Ok");
            return Ok("Version 1");
        }

        [HttpPost]
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
