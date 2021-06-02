﻿using ExampleService.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleService.Application.Commands.AccountCommand
{
    public class TestCommand : IRequest<bool>
    {
        public string Name { get; set; }
        public class TestCommandHandler : IRequestHandler<TestCommand, bool>
        {
            private readonly IAccountService _accountService;
            public TestCommandHandler(IAccountService accountService)
            {
                _accountService = accountService;
            }
            public async Task<bool> Handle(TestCommand request, CancellationToken cancellationToken)
            {
                var s = await _accountService.Login(new LoginCommand());
                var name = request.Name;
                return true;
            }
        }
    }
}
