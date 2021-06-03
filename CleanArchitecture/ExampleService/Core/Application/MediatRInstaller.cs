using ExampleService.Core.Application.Commands.AccountCommand;
using ExampleService.Core.Application.Queries;
using ExampleService.Core.Application.Validations.AccountCommandValidations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ExampleService.Core.Application
{
    public static class MediatRInstaller
    {
        public static void AddMediatR(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient<IValidator<TestCommand>, TestCommandValidator>();
            services.AddQuery();
        }
    }
}
