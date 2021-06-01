using Application.IoC;
using ExampleService.Authorization;
using ExampleService.Behaviours;
using ExampleService.Extensions;
using ExampleService.Infrastructure;
using ExampleService.Infrastructure.Entities;
using ExampleService.Infrastructure.Interface.IRepository;
using ExampleService.Infrastructure.Interface.Repository;
using ExampleService.Infrastructure.Interface.UnitOfWork;
using ExampleService.Services;
using MediatR;
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
            #region DbContext
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<ApplicationDbContext>(opt => opt.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDbContext<ApplicationDbContext>(opts =>
            //{
            //    //opts.UseInMemoryDatabase("ExampleDatabase");
            //    opts.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            //});
            services.AddIdentity<User, Role>(options =>
               {
                   // Cấu hình về Password
                   options.Password.RequireDigit = true; // Không bắt phải có số
                   options.Password.RequireLowercase = true; // Không bắt phải có chữ thường
                   options.Password.RequireNonAlphanumeric = true; // Không bắt ký tự đặc biệt
                   options.Password.RequireUppercase = true; // Không bắt buộc chữ in
                   options.Password.RequiredLength = 8; // Số ký tự tối thiểu của password
                   options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                   // Cấu hình Lockout - khóa user
                   options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                   options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
                   options.Lockout.AllowedForNewUsers = true;

                   // Cấu hình về User.
                   options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                       "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_@";
                   //options.User.RequireUniqueEmail = true; // Email là duy nhất
               })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
            #endregion
            services.ApplicationRegisterServices(Configuration);
            services.InstallServices();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            services.AddScoped<IJwtToken, JwtToken>();
            services.AddMediatR(Assembly.GetExecutingAssembly());
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
