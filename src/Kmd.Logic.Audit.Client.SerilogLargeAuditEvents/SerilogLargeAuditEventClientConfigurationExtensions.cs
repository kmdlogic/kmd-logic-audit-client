using System;
using Azure.Storage.Blobs;
using Kmd.Logic.Audit.Client.SerilogLargeAuditEvents.AzureBlobOrEventHubCustomSink;
using Serilog;

namespace Kmd.Logic.Audit.Client.SerilogLargeAuditEvents
{
    public static class SerilogLargeAuditEventClientConfigurationExtensions
    {
        public static LoggerConfiguration CreateDefaultAzureBlobOrEventHubConfiguration(this SerilogLargeAuditEventClientConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var configBuilder = new LoggerConfiguration()
                    .Enrich.With(new EventIdEnricher())
                    .Enrich.With(new CreatedDateTimeEnricher())
                    .Enrich.WithProperty(Constants.EventSourceProperty, config.EventSource)
                    .WriteTo.AzureBlobOrEventHub(
                        storageConnectionString: config.StorageConnectionString,
                        storageContainerName: config.StorageContainerName,
                        eventHubConnectionString: config.EventHubConnectionString,
                        eventHubName: config.AuditEventTopic,
                        eventSizeLimitInBytes: config.EventSizeLimitinBytes,
                        formatter: new Serilog.Formatting.Compact.CompactJsonFormatter());

            if (config.EnrichFromLogContext == true)
            {
                configBuilder = configBuilder.Enrich.FromLogContext();
            }

            return configBuilder;
        }

        public static LoggerConfiguration CreateAzureBlobOrEventHubConfiguration(this SerilogLargeAuditEventClientConfiguration config, BlobServiceClient blobServiceClient)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var configBuilder = new LoggerConfiguration()
                    .Enrich.With(new EventIdEnricher())
                    .Enrich.With(new CreatedDateTimeEnricher())
                    .Enrich.WithProperty("_EventSource", config.EventSource)
                    .WriteTo.AzureBlobOrEventHub(
                        eventHubConnectionString: config.EventHubConnectionString,
                        eventHubName: config.AuditEventTopic,
                        eventSizeLimitInBytes: config.EventSizeLimitinBytes,
                        blobServiceClient: blobServiceClient,
                        storageContainerName: config.StorageContainerName,
                        formatter: new Serilog.Formatting.Compact.CompactJsonFormatter());

            if (config.EnrichFromLogContext == true)
            {
                configBuilder = configBuilder.Enrich.FromLogContext();
            }

            return configBuilder;
        }
    }
}
