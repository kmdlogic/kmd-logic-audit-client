using System;
using System.Net.Http;
using Serilog;
using Serilog.Core;

namespace Kmd.Logic.Audit.Client.SerilogSeq
{
    public class SerilogSeqAuditClient : IAudit, IDisposable
    {
        private readonly Logger disposableLogger;
        private readonly SerilogLoggerAudit audit;

        public SerilogSeqAuditClient(SerilogSeqAuditClientConfiguration config, HttpMessageHandler messageHandler = null)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var configuBuilder = new LoggerConfiguration()
                .AuditTo.Seq($"{config.ServerUrl}", apiKey: config.ApiKey, messageHandler: messageHandler, compact: true);

            if (config.EnrichFromLogContext == true)
            {
                configuBuilder = configuBuilder.Enrich.FromLogContext();
            }

            this.disposableLogger = configuBuilder.CreateLogger();
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
