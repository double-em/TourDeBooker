using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application;
using Infrastructure;
using Infrastructure.Services.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BackOfficeService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config
                        .SetBasePath(Environment.CurrentDirectory)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                            optional: true);

                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();

                    var messageClientSettingsConfig = hostContext.Configuration.GetSection("RabbitMq");
                    var messageClientSettings = messageClientSettingsConfig.Get<RabbitMqConfiguration>();
                    services.Configure<RabbitMqConfiguration>(messageClientSettingsConfig);

                    services.AddApplication();
                    services.AddInfrastructure(hostContext.Configuration);

                    if (messageClientSettings.Enabled)
                    {
                        services.AddHostedService<AdminService.AdminService>();

                    }
                });
    }
}