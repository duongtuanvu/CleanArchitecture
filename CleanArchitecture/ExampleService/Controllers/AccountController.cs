using Core.Extensions;
using ExampleService.Core.Application.Commands.AccountCommand;
using ExampleService.Infrastructure.Entities;
using ExampleService.Core.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExampleService.Core.Application.Queries.AccountQuery;

namespace ExampleService.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMediator _mediator;
        private readonly AccountQuery _accountQuery;
        public AccountController(IAccountService accountService, IMediator mediator, AccountQuery accountQuery)
        {
            _accountService = accountService;
            _mediator = mediator;
            _accountQuery = accountQuery;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] AccountSearch search)
        {
            var result = await _accountQuery.List(search);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Token([FromBody] TestCommand request)
        {
            await _mediator.Send(request);
            return Ok(new ResponseExtension(data: request.Name));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Token([FromBody] LoginCommand request)
        {
            var token = await _accountService.Login(request);
            return Ok(new ResponseExtension(data: token));
        }
    }
}
