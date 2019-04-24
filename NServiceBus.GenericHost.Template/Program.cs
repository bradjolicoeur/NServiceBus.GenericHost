﻿namespace NServiceBus.GenericHost.Template
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using NServiceBus;
    using System;
    using System.Threading.Tasks;
    using ProducerEndpoint;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System.IO;
    using NServiceBus.GenericHost.Template.Configuration;

    class Program
    {

        public const string EndpointName = "Producer";

        static async Task Main()
        {

            //Set console title
            Console.Title = EndpointName;

            var host = new HostBuilder()
                        .ConfigureHostConfiguration(configHost =>
                        {
                            configHost.SetBasePath(Directory.GetCurrentDirectory());
                            configHost.AddJsonFile("hostsettings.json", optional: true);
                        })
                        .ConfigureAppConfiguration((hostContext, configApp) =>
                        {
                            configApp.AddJsonFile("appsettings.json", optional: true);
                            configApp.AddJsonFile(
                                $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                                optional: true);
                        })
                        .ConfigureLogging((hostContext, configLogging) =>
                        {
                            configLogging.AddConsole();
                            configLogging.AddDebug();
                        })
                        .ConfigureServices((hostContext, services) =>
                        {

                            //Configure NSB Endpoint
                            services.AddSingleton<EndpointConfiguration>(EndpointConfigurations.ConfigureNSB(services, EndpointName));

                            services.AddHostedService<HostedService>();

                        })
                        .UseConsoleLifetime()
                        .Build();

            await host.RunAsync();

        }

    }
}
