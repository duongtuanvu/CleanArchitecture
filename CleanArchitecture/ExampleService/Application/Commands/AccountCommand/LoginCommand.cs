﻿using ExampleService.DTOes;
using ExampleService.Extensions;
using ExampleService.Infrastructure.Entities;
using ExampleService.Infrastructure.Interface.UnitOfWork;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleService.Application.Commands.AccountCommand
{
    public class LoginCommand
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}