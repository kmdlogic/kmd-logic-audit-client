using System;
using System.Collections.Generic;
using System.Text;
using Azure.Storage.Blobs;
using Serilog;
using Serilog.Configuration;
using Serilog.Formatting;

namespace Kmd.Logic.CustomSink.AzureBlobOrEventHub
{
    public static class LoggerConfigurationAzureBlobOrEventHubExtensions
    {
        public static LoggerConfiguration AzureBlobOrEventHub(
              this LoggerSinkConfiguration loggerConfiguration,
              string connectionString,
              string storageContainerName = null,
            string storageBlobName = null,
              ITextFormatter formatter = null)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            return loggerConfiguration.Sink(new AzureBlobOrEventHubSink(blobServiceClient, formatter, storageContainerName, storageBlobName));
        }
    }
}
