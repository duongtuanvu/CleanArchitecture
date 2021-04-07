using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Json;
using System;
using System.IO;

namespace Application.Logger
{
    public static class LoggerExtension
    {
        private static IConfiguration _configuration;

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
                .WriteTo.Logger(c =>
                    c.Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Error)
                    .WriteTo.File(path: ".\\Logs\\Errors\\error-.txt", outputTemplate: "[{Timestamp:dd/MM/yyyy HH:mm:ss}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day))
                .WriteTo.Logger(c =>
                    c.Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Information)
                    .WriteTo.File(path: ".\\Logs\\Informations\\log-.txt", outputTemplate: "[{Timestamp:dd/MM/yyyy HH:mm:ss}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day))
                //.ReadFrom.Configuration(_configuration)
                .CreateLogger();
        }

        public static void Information(string message)
        {
            Log.Logger.Information(message);
        }

        public static void Error(Exception ex)
        {
            Log.Logger.Fatal(ex, "The Application failed to start.");
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
