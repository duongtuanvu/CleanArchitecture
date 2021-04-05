using Application.IoC;
using Data.Repository;
using Data.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoC
{
    public static class DependencyContainer
    {
        public static void RegisterServices(this IServiceCollection service)
        {
            service.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            service.AddScoped<IUnitOfWork, UnitOfWork>();
            service.ApplicationRegisterServices();
        }
    }
}
