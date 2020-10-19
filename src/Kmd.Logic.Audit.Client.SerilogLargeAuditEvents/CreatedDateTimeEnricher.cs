using System;
using Serilog.Core;
using Serilog.Events;

namespace Kmd.Logic.Audit.Client.SerilogLargeAuditEvents
{
    internal class CreatedDateTimeEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("_CreatedDateTime", DateTimeOffset.UtcNow));
        }
    }
}
