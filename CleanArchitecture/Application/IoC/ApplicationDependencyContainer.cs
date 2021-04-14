using Application.Features.ExampleFeature.Commands;
using Application.Features.ExampleFeature.Queries;
using Application.Features.ExampleFeature.Validations;
using Application.Behaviours;
using Application.Extensions;
using DinkToPdf;
using DinkToPdf.Contracts;
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
using Application.RestSharpClients;

namespace Application.IoC
{
    public static class ApplicationDependencyContainer
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
            service.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            service.AddTransient<IRestSharpClient, RestSharpClient>();
            service.AddHttpContextAccessor();
            service.AddAutoMapper(typeof(ExampleDto)
); ;
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
            service.AddTransient<IValidator<CreateCommand>, CreateCommandValidator>();
            service.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }
    }
}
