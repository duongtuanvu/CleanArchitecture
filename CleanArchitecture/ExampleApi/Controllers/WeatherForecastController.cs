using Application.Features.ExampleFeature.Commands;
using Application.Features.ExampleFeature.Queries;
using Application.Token;
using Domain.Interface;
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
        private readonly IUnitOfWork _uow;
        private readonly IExampleQuery _exampleQuery;
        private readonly ILogger<WeatherForecastController> _logger;
        public WeatherForecastController(IExampleQuery exampleQuery, ILogger<WeatherForecastController> logger, IMediator mediat, IJwtToken jwtToken, IUnitOfWork uow)
        {
            _exampleQuery = exampleQuery;
            _logger = logger;
            _mediat = mediat;
            _jwtToken = jwtToken;
            _uow = uow;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _exampleQuery.Get(id);
            return Ok(new Application.Response.Response(data: result));
            //return _jwtToken.GenerateToken();
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
