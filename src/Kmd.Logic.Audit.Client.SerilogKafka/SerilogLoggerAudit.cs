using System;
using Serilog;

namespace Kmd.Logic.Audit.Client.SerilogKafka
{
    internal class SerilogLoggerAudit : IAudit
    {
        private readonly ILogger logger;

        public SerilogLoggerAudit(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Write(string messageTemplate, params object[] propertyValues)
        {
            this.logger.Information(messageTemplate, propertyValues);
        }

        public IAudit ForContext(string propertyName, object value, bool captureObjectStructure = false)
        {
            return new SerilogLoggerAudit(
                this.logger.ForContext(propertyName, value, destructureObjects: captureObjectStructure));
        }
    }
}
