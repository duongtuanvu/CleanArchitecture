using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Core.Helpers
{
    public static class HelperInstaller
    {
        public static void AddHelper(this IServiceCollection services, IConfiguration configuration)
        {
            //services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            //services.AddScoped<IJwtToken, JwtToken>();
        }
    }
}
