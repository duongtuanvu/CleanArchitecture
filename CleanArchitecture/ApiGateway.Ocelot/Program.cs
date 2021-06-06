using Core.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiGateway.Ocelot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LoggerExtension.Configure();
            try
            {
                LoggerExtension.Information("Application Starting.");
                CreateHostBuilder(args).Build().Run();
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
            //CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSeriLog()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile(Path.Combine("configuration.json"));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
