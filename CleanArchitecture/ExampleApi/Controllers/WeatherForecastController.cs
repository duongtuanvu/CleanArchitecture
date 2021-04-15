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
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Hosting;

namespace ExampleApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _mediat;
        private readonly IJwtToken _jwtToken;
        private readonly IExampleQuery _exampleQuery;
        private readonly IRestSharpClient _restSharpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public WeatherForecastController(IExampleQuery exampleQuery, IMediator mediat, IJwtToken jwtToken, IRestSharpClient restSharpClient, IWebHostEnvironment webHostEnvironment)
        {
            _exampleQuery = exampleQuery;
            _mediat = mediat;
            _jwtToken = jwtToken;
            _restSharpClient = restSharpClient;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] SearchExtension search)
        {
            return Ok(await _mediat.Send(new ListQuery(search)));
            //var result = await _exampleQuery.List(search);
            //return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _mediat.Send(new GetQuery(id)));
        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Create(CreateCommand request)
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

        [HttpGet]
        [ApiVersion("2.0")]
        public IActionResult Get2([FromServices] IWebHostEnvironment webHostEvironment)
        {
            var debug = webHostEvironment.ContentRootPath;
            var lst = new List<ExampleDto>()
            {
                new ExampleDto(){Id = 1, Name = "1", Age = 1},
                new ExampleDto(){Id = 2, Name = "2", Age = 2},
                new ExampleDto(){Id = 3, Name = "3", Age = 3},
                new ExampleDto(){Id = 4, Name = "4", Age = 4},
                new ExampleDto(){Id = 5, Name = "5", Age = 5},
                new ExampleDto(){Id = 6, Name = "6", Age = 22},
                new ExampleDto(){Id = 7, Name = "7", Age = 22},
                new ExampleDto(){Id = 8, Name = "8", Age = 22},
                new ExampleDto(){Id = 9, Name = "9", Age = 22},
                new ExampleDto(){Id = 10, Name = "10", Age = 22},
            };

            Stopwatch toList = new Stopwatch();
            toList.Start();
            var list = lst.ToList();
            toList.Stop();

            Stopwatch toDic = new Stopwatch();
            toDic.Start();
            var dic = lst.ToDictionary(x => x.Id);
            toDic.Stop();

            Stopwatch toLookup = new Stopwatch();
            toLookup.Start();
            var lookup = lst.ToLookup(x => x.Age);
            toLookup.Stop();

            Stopwatch toGroupby = new Stopwatch();
            toGroupby.Start();
            var groupby = lst.GroupBy(x => x.Age);
            toGroupby.Stop();
            return Ok($"toDic: {toDic.Elapsed} \\r\\n toLookup: {toLookup.Elapsed} \\r\\n toGroupby: {toGroupby.Elapsed} \\r\\n toList: {toList.Elapsed}");
        }
    }
}
