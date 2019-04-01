using System;
using System.Net.Http;
using Serilog;
using Serilog.Core;

namespace Kmd.Logic.Audit.Client.SerilogAzureEventHubs
{
    public class SerilogAzureEventHubsAuditClient : IAudit, IDisposable
    {
        private readonly Logger disposableLogger;
        private readonly SerilogLoggerAudit audit;

        public SerilogAzureEventHubsAuditClient(SerilogAzureEventHubsAuditClientConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var configBuilder = new LoggerConfiguration()
                .AuditTo.AzureEventHub(
                    formatter: new Serilog.Formatting.Compact.RenderedCompactJsonFormatter(),
                    connectionString: config.ConnectionString,
                    eventHubName: config.AuditEventTopic)
                ;

            if (config.EnrichFromLogContext == true)
            {
                configBuilder = configBuilder.Enrich.FromLogContext();
            }

            this.disposableLogger = configBuilder.CreateLogger();
            this.audit = new SerilogLoggerAudit(this.disposableLogger);
        }

        public void Write(string messageTemplate, params object[] propertyValues)
        {
            this.audit.Write(messageTemplate, propertyValues);
        }

        public IAudit ForContext(string propertyName, object value, bool captureObjectStructure = false)
        {
            return this.audit.ForContext(propertyName, value, captureObjectStructure);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.disposableLogger.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
