using ExampleService.Core.Behaviours;
using ExampleService.Core.Helpers;
using ExampleService.Infrastructure.Entities;
using ExampleService.Infrastructure.Interface.IRepository;
using ExampleService.Infrastructure.Interface.Repository;
using ExampleService.Infrastructure.Interface.UnitOfWork;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ExampleService.Core.Helpers.Enums;

namespace ExampleService.Infrastructure
{
    public static class InfrastructureInstaller
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration, ServerType serverType = ServerType.SqlServer)
        {
            switch (serverType)
            {
                case ServerType.SqlServer:
                    services.AddDbContext<ApplicationDbContext>(opts =>
                    {
                        //opts.UseInMemoryDatabase("ExampleDatabase");
                        opts.UseSqlServer(configuration.GetConnectionString("SqlServerConnection"));
                    });
                    break;
                case ServerType.Postgres:
                    services.AddDbContext<ApplicationDbContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));
                    break;
                default:
                    break;
            }
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
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
