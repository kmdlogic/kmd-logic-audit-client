using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kmd.Logic.Audit.Client.SerilogSeq;
using Serilog.Context;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace Kmd.Logic.Audit.Client.Tests
{
    public class SerilogSeqAuditClientTests
    {
        private readonly ITestOutputHelper output;

        public SerilogSeqAuditClientTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        private class CustomHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> handler;

            public CustomHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
            {
                this.handler = handler;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return this.handler(request);
            }
        }

        [Fact]
        public async Task SendsAPostHttpRequestWithTheCorrectCompactJsonDataImmediately()
        {
            HttpRequestMessage receivedHttpRequest = null;
            var messageHandler = new CustomHttpMessageHandler(request =>
            {
                receivedHttpRequest = request;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Created));
            });

            var client = new SerilogSeqAuditClient(
                new SerilogSeqAuditClientConfiguration
                {
                    ServerUrl = new Uri("http://localhost:5341/"),
                    ApiKey = null,
                    EnrichFromLogContext = true,
                }, messageHandler: messageHandler);

            using (LogContext.PushProperty("LogContext", "success"))
            {
                client
                    .ForContext("ForContext", "success")
                    .Write("Hey hey, from {Source}", nameof(AuditTests));
            }

            Assert.NotNull(receivedHttpRequest);
            Assert.Equal(HttpMethod.Post, receivedHttpRequest.Method);
            Assert.Equal(new Uri("http://localhost:5341/api/events/raw"), receivedHttpRequest.RequestUri);
            var requestBodyContent = await receivedHttpRequest.Content.ReadAsStringAsync().ConfigureAwait(false);
            this.output.WriteLine(requestBodyContent);

            using (var clefReader =
                new Serilog.Formatting.Compact.Reader.LogEventReader(new StringReader(requestBodyContent)))
            {
                var didRead = clefReader.TryRead(out var deserializedEvent);
                Assert.True(didRead);

                Assert.Equal(LogEventLevel.Information, deserializedEvent.Level);
                Assert.Null(deserializedEvent.Exception);
                Assert.Equal("Hey hey, from {Source}", deserializedEvent.MessageTemplate.Text);

                Assert.Collection(
                    deserializedEvent.Properties,
                    item => Assert.Equal("Source", item.Key),
                    item => Assert.Equal("ForContext", item.Key),
                    item => Assert.Equal("LogContext", item.Key));
            }
        }
    }
}
