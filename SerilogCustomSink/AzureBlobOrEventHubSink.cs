using System;
using System.Collections.Generic;
using System.Text;
using Serilog.Core;
using Serilog.Events;

namespace Kmd.Logic.CustomSink.AzureBlobOrEventHub
{
    public class AzureBlobOrEventHubSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;

        public AzureBlobOrEventHubSink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);
            Console.WriteLine(DateTimeOffset.Now.ToString() + " " + message);
        }
    }
}
