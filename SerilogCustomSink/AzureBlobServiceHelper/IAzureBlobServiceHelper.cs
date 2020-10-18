using System;
using System.Collections.Generic;
using System.Text;
using Serilog.Events;
using Serilog.Formatting;

namespace Kmd.Logic.CustomSink.AzureBlobOrEventHub
{
    public interface IAzureBlobServiceHelper
    {
        IEnumerable<string> PrepareBlobContentForUpload(ITextFormatter textFormatter, IEnumerable<LogEvent> logEvents);
    }
}
