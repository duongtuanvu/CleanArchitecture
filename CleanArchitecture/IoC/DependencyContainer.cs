using Application.IoC;
using Data.Repository;
using Data.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

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
