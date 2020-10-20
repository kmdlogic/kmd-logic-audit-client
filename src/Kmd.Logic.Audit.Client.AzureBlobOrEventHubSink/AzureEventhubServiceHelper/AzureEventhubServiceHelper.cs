using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Azure.EventHubs;
using Serilog.Events;
using Serilog.Formatting;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    public class AzureEventhubServiceHelper : IAzureEventhubServiceHelper
    {
        public EventData PrepareEventhubMessageContent(ITextFormatter textFormatter, LogEvent logEvent)
        {
            byte[] body;
            using (var render = new StringWriter())
            {
                textFormatter.Format(logEvent, render);
                body = Encoding.UTF8.GetBytes(render.ToString());
            }

            var eventHubData = new EventData(body);
            eventHubData.Properties.Add("Type", "SerilogEvent");
            eventHubData.Properties.Add("Level", logEvent.Level.ToString());
            return eventHubData;
        }
    }
}
