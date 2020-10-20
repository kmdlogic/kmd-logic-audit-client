using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Serilog.Events;
using Serilog.Formatting;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    public class AuditEventPayload
    {
        public string MessageTemplate
        {
            get
            {
                return "Event details for event id {0} can be found at {1}";
            }
        }

        public static bool DoesAuditEventPayloadExceedLimit(ITextFormatter textFormatter, LogEvent logEvent, int eventSizeLimit)
        {
            string content;
            using (var render = new StringWriter())
            {
                textFormatter.Format(logEvent, render);
                content = render.ToString();
            }

            if (Encoding.UTF8.GetByteCount(content) > eventSizeLimit)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Transform the logevent to provide different message with blob url property added
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="blobUrl"></param>
        /// <returns></returns>
        public LogEvent AuditEventMessageTransformation(LogEvent logEvent, string blobUrl)
        {
            IList<LogEventProperty> properties = new List<LogEventProperty>();
            foreach (var property in logEvent.Properties)
            {
                LogEventProperty logEventProperty = new LogEventProperty(property.Key, property.Value);
                properties.Add(logEventProperty);
            }

            LogEvent newLogEvent = new LogEvent(
                logEvent.Timestamp,
                logEvent.Level,
                logEvent.Exception,
                new MessageTemplate(string.Format(this.MessageTemplate, logEvent.Properties["_EventId"], blobUrl), logEvent.MessageTemplate.Tokens),
                properties);

            return newLogEvent;
        }
    }
}
