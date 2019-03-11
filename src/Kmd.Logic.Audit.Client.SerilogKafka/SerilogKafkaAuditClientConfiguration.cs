using System;

namespace Kmd.Logic.Audit.Client.SerilogKafka
{
    public class SerilogKafkaAuditClientConfiguration
    {
        // NOTE: this is not done
        // Try looking here maybe?
        // https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-quickstart-kafka-enabled-event-hubs#send-and-receive-messages-with-kafka-in-event-hubs

        public Uri ServerUrl { get; set; }

        public string ApiKey { get; set; }

        public bool? EnrichFromLogContext { get; set; }
    }
}
