﻿using System;
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

            if (string.IsNullOrWhiteSpace(eventHubConnectionString))
            {
                throw new ArgumentNullException(nameof(eventHubConnectionString));
            }

            if (string.IsNullOrWhiteSpace(eventHubName))
            {
                throw new ArgumentNullException(nameof(eventHubName));
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var eventHubConnectionstringBuilder = new EventHubsConnectionStringBuilder(eventHubConnectionString)
            {
                EntityPath = eventHubName
            };

            var eventHubclient = EventHubClient.CreateFromConnectionString(eventHubConnectionstringBuilder.ToString());

            var blobServiceClient = new BlobServiceClient(storageConnectionString);

            return loggerConfiguration.Sink(new AzureBlobOrEventHubCustomSink(blobServiceClient, eventHubclient, formatter, eventSizeLimitInBytes, storageContainerName));
        }

        public static LoggerConfiguration AzureBlobOrEventHub(
              this LoggerSinkConfiguration loggerConfiguration,
              string eventHubConnectionString,
              string eventHubName,
              int eventSizeLimitInBytes,
              BlobServiceClient blobServiceClient,
              string storageContainerName = null,
              ITextFormatter formatter = null)
        {
            if (loggerConfiguration == null)
            {
                throw new ArgumentNullException(nameof(loggerConfiguration));
            }

            if (string.IsNullOrWhiteSpace(eventHubConnectionString))
            {
                throw new ArgumentNullException(nameof(eventHubConnectionString));
            }

            if (string.IsNullOrWhiteSpace(eventHubName))
            {
                throw new ArgumentNullException(nameof(eventHubName));
            }

            if (blobServiceClient == null)
            {
                throw new ArgumentNullException(nameof(blobServiceClient));
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var eventHubConnectionstringBuilder = new EventHubsConnectionStringBuilder(eventHubConnectionString)
            {
                EntityPath = eventHubName
            };

            var eventHubclient = EventHubClient.CreateFromConnectionString(eventHubConnectionstringBuilder.ToString());

            return loggerConfiguration.Sink(new AzureBlobOrEventHubCustomSink(blobServiceClient, eventHubclient, formatter, eventSizeLimitInBytes, storageContainerName));
        }
    }
}
