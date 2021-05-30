using Application.Behaviours;
using Application.Extensions;
using DinkToPdf;
using DinkToPdf.Contracts;
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
        public static void ApplicationRegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.RegisterServices();
            services.RegisterValidations();
            services.RegisterAuthentication(configuration);
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddMediatR(Assembly.GetExecutingAssembly());
            //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
            services.AddScoped<IJwtToken, JwtToken>();
            //services.AddTransient<IExampleQuery, ExampleQuery>();
            //services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            //services.AddTransient<IRestSharpClient, RestSharpClient>();
            services.AddHttpContextAccessor();
        }

        public static void RegisterValidations(this IServiceCollection services)
        {
            services.AddMvc(opts =>
            {
                opts.Filters.Add<FilterBehaviour>();
                opts.Filters.Add<ExceptionBehaviour>();
            });
                //.AddFluentValidation();
            //services.AddTransient<IValidator<CreateExampleCommand>, CreateExampleCommandValidator>();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        public static void RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            #region Add jwt authen
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer("Bearer", x =>
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
    }
}
