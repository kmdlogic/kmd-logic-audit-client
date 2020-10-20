using System;
using System.Collections.Generic;
using System.Text;
using Azure.Storage.Blobs;
using Microsoft.Azure.EventHubs;
using Serilog;
using Serilog.Configuration;
using Serilog.Formatting;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    public static class LoggerConfigurationAzureBlobOrEventHubExtensions
    {
        public static LoggerConfiguration AzureBlobOrEventHub(
              this LoggerSinkConfiguration loggerConfiguration,
              string connectionString,
              string eventConnectionString,
              string eventHubName,
              int eventSizeLimitInBytes = 256 * 1024,
              string storageContainerName = null,
            string storageBlobName = null,
              ITextFormatter formatter = null)
        {
            if (loggerConfiguration == null)
            {
                throw new ArgumentNullException(nameof(loggerConfiguration));
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            if (string.IsNullOrWhiteSpace(eventConnectionString))
            {
                throw new ArgumentNullException("eventConnectionString");
            }

            if (string.IsNullOrWhiteSpace(eventHubName))
            {
                throw new ArgumentNullException("eventHubName");
            }

            var connectionstringBuilder = new EventHubsConnectionStringBuilder(eventConnectionString)
            {
                EntityPath = eventHubName
            };

            var eventHubclient = EventHubClient.CreateFromConnectionString(connectionstringBuilder.ToString());

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            return loggerConfiguration.Sink(new AzureBlobOrEventHubSink(blobServiceClient, formatter, eventHubclient, eventSizeLimitInBytes, storageContainerName, storageBlobName));
        }
    }
}
