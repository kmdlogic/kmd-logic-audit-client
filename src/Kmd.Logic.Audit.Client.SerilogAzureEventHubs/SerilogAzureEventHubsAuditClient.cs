using System;
using Serilog.Core;

namespace Kmd.Logic.Audit.Client.SerilogAzureEventHubs
{
    public class SerilogAzureEventHubsAuditClient : IAudit, IDisposable
    {
        private readonly Logger logger;
        private readonly bool disposeLogger;
        private readonly SerilogLoggerAudit audit;

        public SerilogAzureEventHubsAuditClient(SerilogAzureEventHubsAuditClientConfiguration config)
        {
            this.logger = config.DefaultConfiguration().CreateLogger();
            this.disposeLogger = true;
            this.audit = new SerilogLoggerAudit(this.logger);
        }

        public SerilogAzureEventHubsAuditClient(Logger logger, bool disposeLogger = true)
        {
            this.logger = logger;
            this.disposeLogger = disposeLogger;
            this.audit = new SerilogLoggerAudit(this.logger);
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
            if (disposing && this.disposeLogger)
            {
                this.logger.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
