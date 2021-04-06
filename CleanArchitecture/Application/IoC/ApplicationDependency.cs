using Application.Features.ExampleFeature.Commands;
using Application.Features.ExampleFeature.Validations;
using Application.Pipelines;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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
