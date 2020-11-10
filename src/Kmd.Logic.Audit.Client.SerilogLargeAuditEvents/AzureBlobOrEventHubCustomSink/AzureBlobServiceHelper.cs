using System;
using System.IO;
using Serilog.Events;
using Serilog.Formatting;

namespace Kmd.Logic.Audit.Client.SerilogLargeAuditEvents.AzureBlobOrEventHubCustomSink
{
    public static class AzureBlobServiceHelper
    {
        public static string PrepareBlobContentForUpload(ITextFormatter textFormatter, LogEvent logEvent)
        {
            if (textFormatter == null)
            {
                throw new ArgumentNullException(nameof(textFormatter));
            }

            if (logEvent == null)
            {
                throw new ArgumentNullException(nameof(logEvent));
            }

            string content;

            using (var tempStringWriter = new StringWriter())
            {
                try
                {
                    textFormatter.Format(logEvent, tempStringWriter);
                    tempStringWriter.Flush();
                }
                catch (Exception ex)
                {
                    Serilog.Debugging.SelfLog.WriteLine($"Exception {ex} thrown during logEvent formatting. The log event will be dropped.");
                }

                content = tempStringWriter.ToString();
            }

            return content;
        }
    }
}
