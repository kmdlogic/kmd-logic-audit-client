using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Serilog.Events;
using Serilog.Formatting;

namespace Kmd.Logic.CustomSink.AzureBlobOrEventHub
{
    public class AzureBlobServiceHelper : IAzureBlobServiceHelper
    {
        public IEnumerable<string> PrepareBlobContentForUpload(ITextFormatter textFormatter, IEnumerable<LogEvent> logEvents)
        {
            if (textFormatter == null)
            {
                throw new ArgumentNullException(nameof(textFormatter));
            }

            if (logEvents == null)
            {
                throw new ArgumentNullException(nameof(logEvents));
            }

            List<string> contents = new List<string>();
            foreach (LogEvent logEvent in logEvents)
            {
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
                        continue;
                    }

                    contents.Add(tempStringWriter.ToString());
                }
            }

            return contents;
        }
    }
}
