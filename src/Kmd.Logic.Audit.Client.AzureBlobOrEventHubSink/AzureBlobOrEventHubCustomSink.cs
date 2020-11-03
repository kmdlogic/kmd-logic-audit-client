using System;
using Azure.Storage.Blobs;
using Microsoft.Azure.EventHubs;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.AzureEventHub;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    public class AzureBlobOrEventHubCustomSink : ILogEventSink
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly ITextFormatter textFormatter;
        private readonly int eventSizeLimit;
        private readonly string storageContainerName;
        private readonly AzureEventHubSink azureEventHubSink;

        public AzureBlobOrEventHubCustomSink(
            BlobServiceClient blobServiceClient,
            EventHubClient eventHubClient,
            ITextFormatter textFormatter,
            int eventSizeLimit = 256 * 1024,
            string storageContainerName = "logs")
        {
            this.textFormatter = textFormatter;
            this.blobServiceClient = blobServiceClient;
            this.storageContainerName = storageContainerName;
            this.eventSizeLimit = eventSizeLimit;
            this.azureEventHubSink = new AzureEventHubSink(eventHubClient, textFormatter);
        }

        public void Emit(LogEvent logEvent)
        {
            if (AuditEventPayload.DoesAuditEventPayloadExceedLimit(this.textFormatter, logEvent, this.eventSizeLimit))
            {
                var blobUrl = this.UploadToBlob(logEvent);
                logEvent = AuditEventPayload.AuditEventMessageTransformation(logEvent, blobUrl);
            }

            this.azureEventHubSink.Emit(logEvent);
        }

        /// <summary>
        /// Handles uploading to Azure blob
        /// </summary>
        /// <param name="logEvent">Log event</param>
        /// <returns>blob url</returns>
        private Uri UploadToBlob(LogEvent logEvent)
        {
            var content = AzureBlobServiceHelper.PrepareBlobContentForUpload(this.textFormatter, logEvent);
            var eventId = string.Empty;

            // Get Event Id value and use it as blob name
            logEvent.Properties.TryGetValue("_EventId", out var eventIdValue);
            if (eventIdValue != null)
            {
                eventId = eventIdValue.ToString();
            }

            return AzureBlobServiceProvider.UploadBlob(this.blobServiceClient, this.storageContainerName, eventId, content);
        }
    }
}
