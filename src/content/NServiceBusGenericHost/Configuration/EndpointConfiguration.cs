using Microsoft.Extensions.DependencyInjection;
using System;
using NServiceBus;
using Microsoft.Extensions.Hosting;

namespace NServiceBusGenericHost.Configuration
{
    public static class EndpointConfigurations
    {
        //TODO: this can be moved to a shared assembly for the system
        public static IHostBuilder ConfigureNSB(this IHostBuilder builder, string endpointName)
        {

            var endpointConfiguration = new EndpointConfiguration(endpointName);

            //Config sections left commented for reference, they will require additional NuGet packages to enable
            //These are plugins that should be enabled for production ready code
            #region Monitoring Config...
            //heartbeat configuration, this is to identify when an endpoint is off or unresponsive
            //endpointConfiguration.SendHeartbeatTo(
            //        serviceControlQueue: "particular.servicecontrol",
            //        frequency: TimeSpan.FromSeconds(15),
            //        timeToLive: TimeSpan.FromSeconds(30));

            //metrics configuration for servicepulse 
            //var metrics = endpointConfiguration.EnableMetrics();

            //metrics.SendMetricDataToServiceControl(
            //    serviceControlMetricsAddress: "particular.monitoring",
            //    interval: TimeSpan.FromMinutes(1),
            //    instanceId: "INSTANCE_ID_OPTIONAL");

            //performance counter configuration
            //var performanceCounters = endpointConfiguration.EnableWindowsPerformanceCounters();
            //performanceCounters.EnableSLAPerformanceCounters(TimeSpan.FromSeconds(10));
            #endregion

            //configuring audit queue and error queue
            endpointConfiguration.AuditProcessedMessagesTo("audit"); //copy of message after processing will go here for servicecontroller
            endpointConfiguration.SendFailedMessagesTo("error"); //after specified retries is hit, message will be moved here for alerting and recovery

            //TODO: Add production transport; learning transport is not going to work well in docker container
            // Learning transport in docker container will complain that there is no sln folder
            var transport = endpointConfiguration.UseTransport<LearningTransport>(); //for production ready, replace with Azure Service Bus, RabbitMQ or MSMQ

            //TODO: Add routing
            var routing = transport.Routing();
            //routing.RouteToEndpoint(typeof(ProcessPollingRequest), "PollingRequestSaga");
            //routing.RegisterPublisher(typeof(ICompletedPollingRequest), "PollingRequestSaga");

            //TODO: Add production persistence
            endpointConfiguration.UsePersistence<LearningPersistence>(); //for production ready, replace with nHibernate or Azure Storage Provider

            var conventions = endpointConfiguration.Conventions();
            NSBConventions.ConfigureConventions(conventions); //this will configure message types by the defined conventions

            endpointConfiguration.EnableInstallers(); //This will create transport and peristence objects

            builder.ConfigureServices((context, serviceCollection) =>
            {
                endpointConfiguration.UseContainer<ServicesBuilder>(
                   customizations: customizations =>
                   {
                       customizations.ExistingServices(serviceCollection);
                   });

                serviceCollection.AddSingleton<EndpointConfiguration>(endpointConfiguration);

            });

            return builder;
        }
    }
}
