using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Application.IoC;
using MassTransit;
using System;

namespace ExampleApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();
            #region RegisterServices
            services.ApplicationRegisterServices(Configuration);
            #endregion
            #region DbContext
            //services.AddDbContext<ApplicationDbContext>(opts =>
            //{
            //    //opts.UseInMemoryDatabase("ExampleDatabase");
            //    opts.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            //});

            services.AddDbContext<ApplicationDbContext>(opts =>
            {
                opts.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
            });
            #endregion
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
            #region RabbitMQ
            services.AddMassTransit(x =>
            {
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
                {
                    //config.UseHealthCheck(provider);
                    config.Host(new Uri("rabbitmq://localhost"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                }));
            });
            services.AddMassTransitHostedService();
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials
            app.UseSwagger();
            app.UseSwaggerUI(opts =>
            {
                // build a swagger endpoint for each discovered API version
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    opts.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private OpenApiInfo CreateMetaInfoAPIVersion(ApiVersionDescription apiDescription)
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
