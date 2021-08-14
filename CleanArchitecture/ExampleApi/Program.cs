using Application.Extensions;
using Data.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace ExampleApi
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
                    .MigrateDatabase()
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
