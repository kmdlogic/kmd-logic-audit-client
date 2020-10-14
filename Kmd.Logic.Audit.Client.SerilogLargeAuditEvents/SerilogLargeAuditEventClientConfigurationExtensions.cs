using System;
using Serilog;

namespace Kmd.Logic.Audit.Client.SerilogLargeAuditEvents
{
    public static class SerilogLargeAuditEventClientConfigurationExtensions
    {
        public static LoggerConfiguration CreateDefaultConfiguration(this SerilogLargeAuditEventClientConfiguration config)
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
                    .WriteTo.AzureBlobStorage(
                        connectionString: config.BlobConnectionString,
                        formatter: new Serilog.Formatting.Compact.CompactJsonFormatter(),
                        storageContainerName: config.BlobContainerName)
                ;

            if (config.EnrichFromLogContext == true)
            {
                configBuilder = configBuilder.Enrich.FromLogContext();
            }

            return configBuilder;
        }
    }
}
