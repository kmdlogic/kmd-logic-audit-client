using System;
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
              string storageConnectionString,
              string eventHubConnectionString,
              string eventHubName,
              int eventSizeLimitInBytes,
              string storageContainerName = null,
              ITextFormatter formatter = null)
        {
            if (loggerConfiguration == null)
            {
                throw new ArgumentNullException(nameof(loggerConfiguration));
            }

            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new ArgumentNullException(nameof(storageConnectionString));
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            if (string.IsNullOrWhiteSpace(eventHubConnectionString))
            {
                throw new ArgumentNullException(nameof(eventHubConnectionString));
            }

            if (string.IsNullOrWhiteSpace(eventHubName))
            {
                throw new ArgumentNullException(nameof(eventHubName));
            }

            var eventhubConnectionstringBuilder = new EventHubsConnectionStringBuilder(eventHubConnectionString)
            {
                EntityPath = eventHubName
            };

            var eventHubclient = EventHubClient.CreateFromConnectionString(eventhubConnectionstringBuilder.ToString());

            var blobServiceClient = new BlobServiceClient(storageConnectionString);

            return loggerConfiguration.Sink(new AzureBlobOrEventHubCustomSink(blobServiceClient, eventHubclient, formatter, eventSizeLimitInBytes, storageContainerName));
        }
    }
}
