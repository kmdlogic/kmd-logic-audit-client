using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Audit.Client.SerilogLargeAuditEvents
{
    public static class Constants
    {
        public const string EventIdPropertyEnricher = "_EventId";
        public const string CreatedDateTimePropertyEnricher = "_CreatedDateTime";
        public const string EventSourceProperty = "_EventSource";
    }
}
