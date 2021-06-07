using Core.Extensions;
using ExampleService.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LoggerExtension.Configure();
            try
            {
                LoggerExtension.Information("Application Starting.");
                CreateHostBuilder(args)
                    .Build()
                    .MigrateDatabase().GetAwaiter().GetResult()
                    .Run();
            }
            catch (Exception ex)
            {
                LoggerExtension.Error(ex);
                throw;
            }
            finally
            {
                LoggerExtension.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSeriLog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            ;
    }
}
