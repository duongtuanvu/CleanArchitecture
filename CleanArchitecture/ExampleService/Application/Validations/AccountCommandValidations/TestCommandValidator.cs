﻿using ExampleService.Application.Commands.AccountCommand;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Application.Validations.AccountCommandValidations
{
    public class TestCommandValidator : AbstractValidator<TestCommand>
    {
        public TestCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
