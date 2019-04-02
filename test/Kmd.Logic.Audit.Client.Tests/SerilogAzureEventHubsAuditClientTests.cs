using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kmd.Logic.Audit.Client.SerilogAzureEventHubs;
using Kmd.Logic.Audit.Client.SerilogSeq;
using Serilog.Context;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace Kmd.Logic.Audit.Client.Tests
{
    public class SerilogAzureEventHubsAuditClientTests
    {
        private readonly ITestOutputHelper output;

        public SerilogAzureEventHubsAuditClientTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact(Skip = "No way to test this as of now")]
        public void SendsTheAuditEventEncodedAsCompactJsonDataImmediately()
        {
            var client = new SerilogAzureEventHubsAuditClient(
                new SerilogAzureEventHubsAuditClientConfiguration
                {
                    ConnectionString = "intentionally-invalid",
                    EnrichFromLogContext = true,
                });

            using (LogContext.PushProperty("LogContext", "AValueOnLogContext"))
            {
                client
                    .ForContext("ForContext", "AValueViaForContext")
                    .Write("Hey hey, from {Source}", nameof(SerilogAzureEventHubsAuditClientTests));
            }
        }
    }
}
