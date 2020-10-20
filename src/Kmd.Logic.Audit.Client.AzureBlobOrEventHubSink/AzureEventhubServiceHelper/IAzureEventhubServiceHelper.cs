using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.EventHubs;
using Serilog.Events;
using Serilog.Formatting;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    public interface IAzureEventhubServiceHelper
    {
        EventData PrepareEventhubMessageContent(ITextFormatter textFormatter, LogEvent logEvent);
    }
}
