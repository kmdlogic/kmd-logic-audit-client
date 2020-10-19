using System;
using Serilog.Core;

namespace Kmd.Logic.Audit.Client.SerilogLargeAuditEvents
{
    public class SerilogLargeAuditEventClient : IAudit, IDisposable
    {
        private readonly Logger eventhubLogger;
        private readonly Logger blobLogger;
        private readonly bool disposeLogger;
        private readonly SerilogLoggerAudit audit;
        private readonly SerilogBlobLoggerAudit auditToBlob;

        public SerilogLargeAuditEventClient(SerilogLargeAuditEventClientConfiguration config)
        {
            this.eventhubLogger = config.CreateEventhubConfiguration().CreateLogger();
            this.blobLogger = config.CreateBlobConfiguration().CreateLogger();
            this.disposeLogger = true;
            this.audit = new SerilogLoggerAudit(this.eventhubLogger);
            this.auditToBlob = new SerilogBlobLoggerAudit(this.blobLogger);
        }

        private SerilogLargeAuditEventClient(Logger logger, bool disposeLogger)
        {
            this.eventhubLogger = logger;
            this.blobLogger = logger;
            this.disposeLogger = disposeLogger;
            this.audit = new SerilogLoggerAudit(this.eventhubLogger);
            this.auditToBlob = new SerilogBlobLoggerAudit(this.blobLogger);
        }

        public static SerilogLargeAuditEventClient CreateCustomized(Logger logger, bool disposeLogger = true)
        {
            return new SerilogLargeAuditEventClient(logger, disposeLogger);
        }

        public void Write(string messageTemplate, params object[] propertyValues)
        {
            //this.audit.Write(messageTemplate, propertyValues);
            this.auditToBlob.Write(messageTemplate, propertyValues);
        }

        public IAudit ForContext(string propertyName, object value, bool captureObjectStructure = false)
        {
            return this.audit.ForContext(propertyName, value, captureObjectStructure);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.disposeLogger)
            {
                this.eventhubLogger.Dispose();
                this.blobLogger.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
