using System;
using System.Collections.Generic;
using System.Text;
using Azure.Storage.Blobs;
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

        public AzureBlobOrEventHubSink(
            BlobServiceClient blobServiceClient,
            ITextFormatter textFormatter,
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



        }

        public void Emit(LogEvent logEvent)
        {
            var content = this.azureBlobServiceHelper.PrepareBlobContentForUpload(this.textFormatter, new[] { logEvent });

            // Get Event Id value and use it as blob name
            LogEventPropertyValue eventIdValue;
            logEvent.Properties.TryGetValue("_EventId", out eventIdValue);
            if (eventIdValue != null)
            {
                this.storageBlobName = eventIdValue.ToString();
            }

            this.azureBlobServiceProvider.UploadBlob(this.blobServiceClient, this.storageContainerName, this.storageBlobName, content);
        }
    }
}
