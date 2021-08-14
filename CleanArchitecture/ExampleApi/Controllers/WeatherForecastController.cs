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
using Application.Extensions;
using Application.RestSharpClients;
using Application.Common;
using Microsoft.AspNetCore.Cors;
using MassTransit;
using Domain.Share;
using System;

namespace ExampleApi.Controllers
{
    //[Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _mediat;
        private readonly IJwtToken _jwtToken;
        private readonly IExampleQuery _exampleQuery;
        private readonly IRestSharpClient _restSharpClient;
        private readonly IBus _bus;

        public WeatherForecastController(IBus bus, IExampleQuery exampleQuery, IMediator mediat, IJwtToken jwtToken, IRestSharpClient restSharpClient)
        {
            _bus = bus;
            _exampleQuery = exampleQuery;
            _mediat = mediat;
            _jwtToken = jwtToken;
            _restSharpClient = restSharpClient;
        }

        [AllowAnonymous]
        [HttpGet("~/api/v{version:apiVersion}/auth/token")]
        public string Token()
        {
            return _jwtToken.GenerateToken();
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] SearchExtension search)
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

        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Create(CreateExampleCommand request)
        {
            await _mediat.Send(request);
            return Ok("Version 1");
        }

        [HttpPost("Excel")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            var data = await (new List<ExampleDto>()).ReadDataFromExcelFile<ExampleDto>(file);
            return Ok(data);
        }

        [HttpGet("Excel")]
        public async Task<IActionResult> ExportExcel([FromQuery] SearchExtension search)
        {
            var result = await _exampleQuery.List(search);
            return File(((IEnumerable<ExampleDto>)result.Data).ExportExcel(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "text.xlsx");
        }

        [HttpGet("Pdf")]
        public async Task<IActionResult> ExportPdf([FromQuery] SearchExtension search)
        {
            var result = await _exampleQuery.List(search);
            //var _byte = Pdf.Export<ExampleDto>((IEnumerable<ExampleDto>)result.Data);
            //return File(_byte, "application/pdf", "text.pdf");
            return Ok();
        }

        [HttpPost("rest")]
        public async Task<IActionResult> RestSharp(SearchExtension search)
        {
            var headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJjMGM0OGJiNC1kOTlkLTQ4NDYtODRhMC0wM2RhOWFjOWY3NjkiLCJQZXJtaXNzaW9ucyI6IiIsIklzQWRtaW4iOiJUcnVlIiwibmJmIjoxNjE4Mzc0NTA2LCJleHAiOjE2MTg0MTc3MDYsImlzcyI6ImNyb3duLXgiLCJhdWQiOiJjcm93bi14In0.wMF94LrTJ7uBMK1Q7wxChNWnSXTrg8r2sP8zUWZ_Drc");
            var param = new Dictionary<string, string>();
            //param.Add("id", "02a3b9d7-6242-4a3d-8f0a-6d5a64632b2e");
            param.Add("Keyword", "vud");
            var requestParams = new RequestParams()
            {
                BaseUrl = "http://localhost:62025/",
                Body = new { Id = "02a3b9d7-6242-4a3d-8f0a-6d5a64632b2e", FullName = "test REST" },
                ApiEndPoint = "api/v1/users",
                Method = Constant.PUT,
                Headers = headers,
                Parameters = param,
                isUrlSegment = true
            };
            var result = await _restSharpClient.ExcuteApi(requestParams);
            return Ok(result.Content);
        }

        [HttpPost("publisher")]
        public async Task<IActionResult> Publisher([FromBody] Email email)
        {
            if (email != null)
            {
                email.To = "vu.dt99@3si.vn";
                email.Subject = "Test";
                email.Body = "<h1>Hello world </h1>";
                Uri uri = new Uri("rabbitmq://localhost/EmailQueue");
                var endPoint = await _bus.GetSendEndpoint(uri);
                await endPoint.Send(email);
            }
            return Ok("Publisher done");
        }

        [HttpGet]
        [ApiVersion("2.0")]
        public IActionResult Get2()
        {
            return Ok("Version 2");
        }
    }
}
