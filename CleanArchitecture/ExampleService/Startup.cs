//using Application.Behaviours;
using Application.IoC;
using ExampleService.Core.Application.Commands.AccountCommand;
using ExampleService.Core.Application.Validations;
using ExampleService.Core.Application.Validations.AccountCommandValidations;
using ExampleService.Core.Authorization;
using ExampleService.Core.Behaviours;
using ExampleService.Core.Helpers;
using ExampleService.Infrastructure;
using ExampleService.Infrastructure.Entities;
using ExampleService.Infrastructure.Interface.IRepository;
using ExampleService.Infrastructure.Interface.Repository;
using ExampleService.Infrastructure.Interface.UnitOfWork;
using ExampleService.Core.Services;
using FluentValidation;
//using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static ExampleService.Core.Helpers.Enums;
using ExampleService.Core.Application;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ExampleService
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
            services.AddControllers();
            services.AddInfrastructure(Configuration, ServerType.Postgres);
            services.AddServicesCore(Configuration);
            //services.AddJwtAuthentication(Configuration);
            services.AddApplicationServices();
            services.AddMediatR();
            services.AddHelper(Configuration);

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
    }
}
