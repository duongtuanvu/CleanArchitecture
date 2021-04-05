using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Application.IoC
{
    public static class ApplicationDependency
    {
        public static void ApplicationRegisterServices(this IServiceCollection service)
        {
            service.AddMediatR(Assembly.GetExecutingAssembly());
        }
    }
}
