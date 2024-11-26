using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace ITOps.EndpointConfig
{
    public static class EndpointConfigurationExtensions
    {
        public static EndpointConfiguration ConfigureNServiceBus(
            this IHostApplicationBuilder builder,
            string endpointName,
            Action<RoutingSettings<RabbitMQTransport>> configureRouting = null)
        {
            var endpointConfiguration = new EndpointConfiguration(endpointName);
            
            endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
            endpointConfiguration.Recoverability().Delayed(c => c.NumberOfRetries(0));

            var transport = new RabbitMQTransport(
                RoutingTopology.Conventional(QueueType.Quorum),
                builder.Configuration.GetConnectionString("broker")
            );
            
            var routing = endpointConfiguration.UseTransport(transport);

            endpointConfiguration.UsePersistence<LearningPersistence>();

            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(t =>
                t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Commands") &&
                t.Name.EndsWith("Command"));
            conventions.DefiningEventsAs(t =>
                t.Namespace != null && t.Namespace.StartsWith("Divergent") && t.Namespace.EndsWith("Events") &&
                t.Name.EndsWith("Event"));

            endpointConfiguration.EnableInstallers();
            
            endpointConfiguration.EnableOpenTelemetry();

            configureRouting?.Invoke(routing);
            
            builder.UseNServiceBus(endpointConfiguration);

            return endpointConfiguration;
        }
    }
}