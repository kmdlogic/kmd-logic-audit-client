using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Serilog.Events;
using Serilog.Formatting;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    public class AzureBlobServiceHelper : IAzureBlobServiceHelper
    {
        public string PrepareBlobContentForUpload(ITextFormatter textFormatter, LogEvent logEvent)
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

            using (StringWriter tempStringWriter = new StringWriter())
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
