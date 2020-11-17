using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Kmd.Logic.Audit.Client.SampleLargeAuditEvents
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)

                // Without *some* values in the config, we get a null instance of `ProgramConfig`
                // however if we ensure at least one value is in the config collection, then we get
                // an instance, no matter what the key/value is.
                .AddInMemoryCollection(initialData: new Dictionary<string, string> { { "dummy", "value" }, })
                .Build()
                .Get<ProgramConfig>();

            if (string.IsNullOrEmpty(config?.Ingestion?.ConnectionString))
            {
                Console.WriteLine("You need to specify a connection string (e.g. --Ingestion:ConnectionString=\"test\")");
                return;
            }

            var clientConfig = new Kmd.Logic.Audit.Client.SerilogLargeAuditEvents.SerilogLargeAuditEventClientConfiguration
            {
                EventSource = config.Ingestion.EventSource ?? $"{typeof(Program).Assembly.GetName().Name} on {Environment.MachineName}",
                AuditEventTopic = config.Ingestion.AuditEventTopic,
                EventHubConnectionString = config.Ingestion.ConnectionString,
                EnrichFromLogContext = config.Client.EnrichFromLogContext,
                StorageAccountName = config.Ingestion.BlobAccountName,
                StorageConnectionString = config.Ingestion.BlobConnectionString,
                StorageContainerName = config.Ingestion.BlobContainerName
            };

            // Create data for audit event
            var events = new List<AuditEvent>();
            for (int i = 0; i < 10000; i++)
            {
                var data = new AuditEvent();
                events.Add(data);
            }

            using (var client = new Kmd.Logic.Audit.Client.SerilogLargeAuditEvents.SerilogLargeAuditEventClient(clientConfig))
            {
                var audit = (IAudit)client;
                var name = typeof(Program).Assembly.GetName().Name;
                var version = typeof(Program).Assembly.GetName().Version;

                Console.WriteLine(
                    "Sending {0} ({3} threads) audit events to {1} at {2} and larger events to {4} in {5} as well",
                    config.Ingestion.NumberOfEventsToSend,
                    clientConfig.AuditEventTopic,
                    clientConfig.EventHubConnectionString,
                    config.Ingestion.NumberOfThreads,
                    config.Ingestion.BlobContainerName,
                    config.Ingestion.BlobAccountName);

                var sw = System.Diagnostics.Stopwatch.StartNew();
                var groupId = Guid.NewGuid();

                if (clientConfig.EnrichFromLogContext == true)
                {
                    Console.WriteLine($"The GroupId {groupId} will be attached to each event");
                }

                using (Serilog.Context.LogContext.PushProperty("GroupId", groupId))
                {
                    Enumerable
                        .Range(0, config.Ingestion.NumberOfEventsToSend)
                        .AsParallel()
                        .WithDegreeOfParallelism(config.Ingestion.NumberOfThreads)
                        .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                        .Select(i =>
                        {
                            audit.Write("Sample audit event with {IterationNum} from {Application} v{Version} and data {@Data}", i, name, version, events);

                            var numDividedBy10Or1 = config.Ingestion.NumberOfEventsToSend < 10
                                ? 1
                                : config.Ingestion.NumberOfEventsToSend / 10;

                            if ((i % numDividedBy10Or1) == 0)
                            {
                                Console.WriteLine($"Sent {i} events so far...");
                            }
                            return i;
                        })
                        .ToArray();
                }

                Console.WriteLine("Finished in {0}ms", sw.ElapsedMilliseconds);
            }
        }
    }
}
