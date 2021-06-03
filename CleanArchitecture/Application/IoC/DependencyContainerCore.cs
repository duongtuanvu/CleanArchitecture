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
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using FluentValidation.AspNetCore;
using FluentValidation;
using Application.Features.ExampleFeature.Commands;
using Application.Features.ExampleFeature.Validations;

namespace Application.IoC
{
    public static class DependencyContainerCore
    {
        public static void AddServicesCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwagger();
            services.AddValidations();
            services.AddAuthentication(configuration);
            //services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            //services.AddMediatR(Assembly.GetExecutingAssembly());
            //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
            //services.AddScoped<IJwtToken, JwtToken>();
            //services.AddTransient<IExampleQuery, ExampleQuery>();
            //services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddTransient<IRestSharpClient, RestSharpClient>();
            services.AddHttpContextAccessor();
        }

        public static void AddValidations(this IServiceCollection services)
        {
            services.AddMvc(opts =>
            {
                opts.Filters.Add<FilterBehaviour>();
                opts.Filters.Add<ExceptionBehaviour>();
            })
                .AddFluentValidation();
            //services.AddTransient<IValidator<CreateExampleCommand>, CreateExampleCommandValidator>();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
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
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            #region Api versioning
            services.AddApiVersioning(config =>
            {
                //config.DefaultApiVersion = new ApiVersion(1, 0);
                //config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
            services.AddVersionedApiExplorer(opts =>
            {
                opts.GroupNameFormat = "'v'VVV";
                opts.SubstituteApiVersionInUrl = true;
            });
            #endregion
            #region Swagger
            services.AddSwaggerGen(opts =>
            {
                var service = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (ApiVersionDescription desc in service.ApiVersionDescriptions)
                {
                    opts.SwaggerDoc(desc.GroupName, CreateMetaInfoAPIVersion(desc));
                }

                opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                opts.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });

                //opts.IncludeXmlComments(string.Format(@"{0}\OnionArchitecture.xml", System.AppDomain.CurrentDomain.BaseDirectory));
            });
            #endregion
        }

        private static OpenApiInfo CreateMetaInfoAPIVersion(ApiVersionDescription apiDescription)
        {
            return new OpenApiInfo
            {
                Title = string.Format("Example API {0}", apiDescription.GroupName.ToUpperInvariant()),
                Version = apiDescription.ApiVersion.ToString(),
                Description = "Swagger aides in development across the entire API lifecycle, from design and documentation, to test and deployment.",
            };

        }
    }
}
