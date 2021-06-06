using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;

namespace Core.Extensions
{
    public static class LoggerExtension
    {
        //private static IConfiguration _configuration;

        /// <summary>
        /// Config Serilog
        /// </summary>
        public static void Configure()
        {
            //Read Configuration from appSettings
            //_configuration = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //    .Build();
            //Initialize Logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .WriteTo.Logger(c =>
                    c.Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Error)
                    .WriteTo.File(path: ".\\Logs\\Errors\\error-.txt", outputTemplate: "[{Timestamp:dd/MM/yyyy HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day))
                .WriteTo.Logger(c =>
                    c.Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Information)
                    .WriteTo.File(path: ".\\Logs\\Informations\\log-.txt", outputTemplate: "[{Timestamp:dd/MM/yyyy HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day))
                //.ReadFrom.Configuration(_configuration)
                .CreateLogger();
        }

        public static void Information(string message)
        {
            Log.Logger.Information(message);
        }

        public static void Error(Exception ex)
        {
            Log.Logger.Fatal(ex, "The Core failed to start.");
        }

        public static void CloseAndFlush()
        {
            Log.CloseAndFlush();
        }

        public static IHostBuilder UseSeriLog(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog();
            return hostBuilder;
        }
    }
}
