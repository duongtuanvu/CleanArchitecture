using Application.Features.ExampleFeature.Commands;
using Application.Pipelines;
using Application.Validations.ExamplModelValidations;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Application.IoC
{
    public static class ApplicationDependency
    {
        public static void ApplicationRegisterServices(this IServiceCollection service)
        {
            service.RegisterValidations();
            service.AddMediatR(Assembly.GetExecutingAssembly());
            service.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
        }

        public static void RegisterValidations(this IServiceCollection service)
        {
            service.AddMvc(opts =>
            {
                opts.Filters.Add<FilterBehaviour>();
                opts.Filters.Add<ExceptionBehaviour>();
            })
                .AddFluentValidation();
            service.AddTransient<IValidator<CreateExampleCommand>, CreateExampleCommandValidator>();
        }
    }
}
