using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Application.Logger
{
    public static class LoggerExtension
    {
        private static IConfiguration _configuration;

        public static void Configure()
        {
            //Read Configuration from appSettings
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            //Initialize Logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .CreateLogger();
        }

        public static void Information(string message)
        {
            Log.Logger.Information(message);
        }

        public static void Error(Exception ex)
        {
            Log.Fatal(ex, "The Application failed to start.");
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
