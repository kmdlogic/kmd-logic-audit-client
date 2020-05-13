using System;
using Serilog;

namespace Kmd.Logic.Audit.Client.SerilogAzureEventHubs
{
    public static class SerilogAzureEventHubsAuditClientConfigurationExtensions
    {
        public static LoggerConfiguration DefaultConfiguration(this SerilogAzureEventHubsAuditClientConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var configBuilder = new LoggerConfiguration()
                    .Enrich.With(new EventIdEnricher())
                    .Enrich.With(new CreatedDateTimeEnricher())
                    .Enrich.WithProperty("_EventSource", config.EventSource)
                    .AuditTo.AzureEventHub(
                        formatter: new Serilog.Formatting.Compact.CompactJsonFormatter(),
                        connectionString: config.ConnectionString,
                        eventHubName: config.AuditEventTopic)
                ;

            if (config.EnrichFromLogContext == true)
            {
                configBuilder = configBuilder.Enrich.FromLogContext();
            }

            return configBuilder;
        }
    }
}
