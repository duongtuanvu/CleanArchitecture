using Application.Extensions;
using Application.Features.ExampleFeature.Commands;
using Application.Features.ExampleFeature.Queries;
using Domain.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Commons;

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
        public async Task<IActionResult> List([FromQuery] Search search)
        {
            var result = await _exampleQuery.List(search);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _exampleQuery.Get(id);
            return Ok(result);
            //return _jwtToken.GenerateToken();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateExampleCommand request)
        {
            await _mediat.Send(request);
            return Ok("Version 1");
        }

        [HttpPost("Excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            var result = await Excel.ReadDataFromExcelFile<ExampleDto>(file);
            return Ok(result);
        }

        [HttpGet("Excel")]
        public async Task<IActionResult> ExportExcel([FromQuery] Search search)
        {
            var result = await _exampleQuery.List(search);
            return File(((IEnumerable<ExampleDto>)result.Data).ExportExcel(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "text.xlsx");
        }

        [HttpGet("Pdf")]
        public async Task<IActionResult> ExportPdf([FromQuery] Search search)
        {
            var result = await _exampleQuery.List(search);
            //var _byte = Pdf.Export<ExampleDto>((IEnumerable<ExampleDto>)result.Data);
            //return File(_byte, "application/pdf", "text.pdf");
            return Ok();
        }

        [HttpGet]
        [ApiVersion("2.0")]
        public IActionResult Get2()
        {
            return Ok("Version 2");
        }
    }
}
