using System;
using System.Collections.Generic;
using System.Text;
using Serilog.Events;
using Serilog.Formatting;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    public interface IAzureBlobServiceHelper
    {
        string PrepareBlobContentForUpload(ITextFormatter textFormatter, LogEvent logEvent);
    }
}
