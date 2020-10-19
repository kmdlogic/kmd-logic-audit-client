using System;
using System.Collections.Generic;
using System.Text;
using Azure.Storage.Blobs;
using Microsoft.Azure.EventHubs;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    public class AzureBlobOrEventHubSink : ILogEventSink
    {
        private readonly ITextFormatter textFormatter;
        private readonly BlobServiceClient blobServiceClient;
        private readonly IAzureBlobServiceHelper azureBlobServiceHelper;
        private readonly IAzureBlobServiceProvider azureBlobServiceProvider;
        private readonly string storageContainerName;
        private string storageBlobName;
        private readonly EventHubClient eventHubClient;

        public AzureBlobOrEventHubSink(
            BlobServiceClient blobServiceClient,
            ITextFormatter textFormatter,
            EventHubClient eventHubClient,
            string storageContainerName = null,
            string storageBlobName = null,
            IAzureBlobServiceHelper azureBlobServiceHelper = null,
            IAzureBlobServiceProvider azureBlobServiceProvider = null)
        {
            if (string.IsNullOrEmpty(storageContainerName))
            {
                storageContainerName = "logs";
            }

            if (string.IsNullOrEmpty(storageBlobName))
            {
                storageBlobName = "log";
            }

            this.textFormatter = textFormatter;
            this.blobServiceClient = blobServiceClient;
            this.storageContainerName = storageContainerName;
            this.storageBlobName = storageBlobName;
            this.azureBlobServiceHelper = azureBlobServiceHelper ?? new AzureBlobServiceHelper();
            this.azureBlobServiceProvider = azureBlobServiceProvider ?? new AzureBlobServiceProvider();
            this.eventHubClient = eventHubClient;
        }

        public void Emit(LogEvent logEvent)
        {
            var content = this.azureBlobServiceHelper.PrepareBlobContentForUpload(this.textFormatter, logEvent);

            // Get Event Id value and use it as blob name
            LogEventPropertyValue eventIdValue;
            logEvent.Properties.TryGetValue("_EventId", out eventIdValue);
            if (eventIdValue != null)
            {
                this.storageBlobName = eventIdValue.ToString();
            }

            this.azureBlobServiceProvider.UploadBlob(this.blobServiceClient, this.storageContainerName, this.storageBlobName, content);

            byte[] body;
            body = Encoding.UTF8.GetBytes(content);
            var eventHubData = new EventData(body);
            eventHubData.Properties.Add("Type", "SerilogEvent");
            eventHubData.Properties.Add("Level", logEvent.Level.ToString());
            this.eventHubClient.SendAsync(eventHubData, Guid.NewGuid().ToString()).GetAwaiter().GetResult();
        }
    }
}
