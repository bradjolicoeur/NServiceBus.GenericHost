using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBusGenericHost
{
    internal class HostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly EndpointConfiguration _endpointConfiguration;

        private IEndpointInstance EndpointInstance { get; set; }

        public HostedService(ILogger<HostedService> logger, EndpointConfiguration endpointConfiguration)
        {
            _logger = logger;
            _endpointConfiguration = endpointConfiguration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service is starting.");

            //Start NSB Endpoint
            EndpointInstance = await Endpoint.Start(_endpointConfiguration);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service is stopping.");

            if (EndpointInstance != null)
            { await EndpointInstance.Stop().ConfigureAwait(false); }
        }

    }
}

