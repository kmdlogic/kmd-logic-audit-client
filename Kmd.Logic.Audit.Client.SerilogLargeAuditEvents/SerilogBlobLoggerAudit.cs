using System;
using Serilog;

namespace Kmd.Logic.Audit.Client.SerilogLargeAuditEvents
{
    internal class SerilogBlobLoggerAudit : IAudit
    {
        private readonly ILogger logger;

        public SerilogBlobLoggerAudit(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Write(string messageTemplate, params object[] propertyValues)
        {
            this.logger.Information(messageTemplate, propertyValues);
        }

        public IAudit ForContext(string propertyName, object value, bool captureObjectStructure = false)
        {
            return new SerilogBlobLoggerAudit(
                this.logger.ForContext(propertyName, value, destructureObjects: captureObjectStructure));
        }
    }
}
