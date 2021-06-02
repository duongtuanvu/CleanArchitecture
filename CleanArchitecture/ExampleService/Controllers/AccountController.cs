using Application.Extensions;
using ExampleService.Application.Commands.AccountCommand;
using ExampleService.Infrastructure.Entities;
using ExampleService.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMediator _mediator;
        public AccountController(IAccountService accountService, IMediator mediator)
        {
            _accountService = accountService;
            _mediator = mediator;
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
