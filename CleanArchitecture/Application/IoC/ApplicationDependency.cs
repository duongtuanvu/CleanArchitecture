using Application.Features.ExampleFeature.Commands;
using Application.Features.ExampleFeature.Queries;
using Application.Features.ExampleFeature.Validations;
using Application.Pipelines;
using Application.Token;
using FluentValidation;
using FluentValidation.AspNetCore;
using IoC;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Reflection;
using System.Text;

namespace Application.IoC
{
    public static class ApplicationDependency
    {
        public static void ApplicationRegisterServices(this IServiceCollection service, IConfiguration configuration)
        {
            service.RegisterServices();
            service.RegisterValidations();
            service.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            service.AddMediatR(Assembly.GetExecutingAssembly());
            service.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
            service.AddScoped<IJwtToken, JwtToken>();
            service.AddTransient<IExampleQuery, ExampleQuery>();
            #region Add jwt authen
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            service.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
                    ValidAudience = jwtSettings.Audience,
                    ValidateAudience = true,
                };
            });
            #endregion
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
            service.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }
    }
}
