using System;
using System.Collections.Generic;
using System.Text;
using Serilog;
using Kmd.Logic.CustomSink.AzureBlobOrEventHub;

namespace Kmd.Logic.Audit.Client.SerilogLargeAuditEvents
{
    public static class SerilogLargeAuditEventClientBlobConfigurationExtensions
    {
        public static LoggerConfiguration CreateBlobConfiguration(this SerilogLargeAuditEventClientConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            //var configBuilder = new LoggerConfiguration()
            //        .Enrich.With(new EventIdEnricher())
            //        .Enrich.With(new CreatedDateTimeEnricher())
            //        .Enrich.WithProperty("_EventSource", config.EventSource)
            //        .WriteTo.AzureBlobStorage(
            //            connectionString: config.BlobConnectionString,
            //            formatter: new Serilog.Formatting.Compact.CompactJsonFormatter(),
            //            storageContainerName: config.BlobContainerName);

            var configBuilder = new LoggerConfiguration()
                    .Enrich.With(new EventIdEnricher())
                    .Enrich.With(new CreatedDateTimeEnricher())
                    .Enrich.WithProperty("_EventSource", config.EventSource)
                    .WriteTo.AzureBlobOrEventHub(
                        connectionString: config.BlobConnectionString,
                        formatter: new Serilog.Formatting.Compact.CompactJsonFormatter());

            if (config.EnrichFromLogContext == true)
            {
                configBuilder = configBuilder.Enrich.FromLogContext();
            }

            return configBuilder;
        }
    }
}
