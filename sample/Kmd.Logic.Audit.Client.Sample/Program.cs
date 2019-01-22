using System;
using System.Collections.Generic;
using Kmd.Logic.Audit.Client.SerilogSeq;
using Microsoft.Extensions.Configuration;

namespace Kmd.Logic.Audit.Client.Sample
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

            IAudit audit = new SerilogSeqAuditClient(
                new SerilogSeqAuditClientConfiguration
                {
                    ServerUrl = config.Ingestion.Endpoint,
                    ApiKey = config.Ingestion.ApiKey,
                    EnrichFromLogContext = config.Client.EnrichFromLogContext,
                });

            var name = typeof(Program).Assembly.GetName().Name;
            var version = typeof(Program).Assembly.GetName().Version;

            Console.WriteLine("Sending audit event to {0}", config.Ingestion.Endpoint);

            using (Serilog.Context.LogContext.PushProperty("LogContext1", Guid.NewGuid()))
            {
                audit
                    .ForContext("AuditForContext1", Guid.NewGuid())
                    .ForContext("StartArgs", args)
                    .Write("Hello from {Application} v{Version}", name, version);
            }
        }
    }
}
