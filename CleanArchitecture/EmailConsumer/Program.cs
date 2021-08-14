using Application.Extensions;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmailConsumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            LoggerExtension.Configure();
            try
            {
                LoggerExtension.Information("Application Starting.");
                CreateHostBuilder(args)
                    .Build()
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
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.Configure<EmailSettings>(hostContext.Configuration.GetSection("EmailSettings"));
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<EmailConsumer>();
                        x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                        {
                            //cfg.UseHealthCheck(provider);
                            cfg.Host(new Uri("rabbitmq://localhost"), h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });
                            cfg.ReceiveEndpoint("EmailQueue", ep =>
                            {
                                ep.PrefetchCount = 16;
                                ep.UseMessageRetry(r => r.Interval(2, 100));
                                ep.ConfigureConsumer<EmailConsumer>(provider);
                            });
                        }));
                    });
                    services.AddMassTransitHostedService();
                });
    }
}
