using Application.Features.ExampleFeature.Commands;
using Data.UnitOfWork;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _mediat;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<WeatherForecastController> _logger;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IUnitOfWork uow, IMediator mediat)
        {
            _logger = logger;
            _uow = uow;
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
