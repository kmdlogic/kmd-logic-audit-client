using System;
using System.Net.Http;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Kafka;

namespace Kmd.Logic.Audit.Client.SerilogKafka
{
    public class SerilogKafkaAuditClient : IAudit, IDisposable
    {
        private readonly Logger disposableLogger;
        private readonly SerilogLoggerAudit audit;

        public SerilogKafkaAuditClient(SerilogKafkaAuditClientConfiguration config, HttpMessageHandler messageHandler = null)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var configBuilder = new LoggerConfiguration()
                // .AuditTo.Seq($"{config.ServerUrl}", apiKey: config.ApiKey, messageHandler: messageHandler, compact: true);
                // TODO: .AuditTo.Kafka(TODO)
                // .WriteTo.Kafka(options: new Serilog.)
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
