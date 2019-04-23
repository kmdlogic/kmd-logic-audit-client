using System;

namespace Kmd.Logic.Audit.Client.Sample
{
    public class ProgramConfigIngestion
    {
        public int NumberOfEventsToSend { get; set; } = 1000;

        public string ConnectionString { get; set; }

        public string AuditEventTopic { get; set; } = "audit";

        public string EventSource { get; set; }
    }
}
