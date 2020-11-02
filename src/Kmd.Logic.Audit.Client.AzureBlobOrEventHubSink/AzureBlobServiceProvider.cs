using System;
using System.IO;
using System.Text;
using Azure.Storage.Blobs;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    public static class AzureBlobServiceProvider
    {
        public const string PathDivider = "/";

        /// <summary>
        /// Uploads blob and return the blob url
        /// </summary>
        /// <param name="blobServiceClient">Blob service client</param>
        /// <param name="blobContainerName">Container name</param>
        /// <param name="eventId">Event id from log context</param>
        /// <param name="content">Content to be uploaded</param>
        /// <returns>Blob url</returns>
        public static Uri UploadBlob(BlobServiceClient blobServiceClient, string blobContainerName, string eventId, string content)
        {
            // Get a reference to a blob
            var blobClient = GetBlobClient(blobServiceClient, blobContainerName, eventId);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                try
                {
                    blobClient.Upload(stream);
                    return blobClient.Uri;
                }
                catch (Exception ex)
                {
                    Serilog.Debugging.SelfLog.WriteLine($"Exception {ex} thrown while trying to upload blob.");
                    return null;
                }
            }
        }

        /// <summary>
        /// Check if container or blob exists or not and return a blob client object accordingly
        /// </summary>
        /// <param name="blobServiceClient">Blob service client</param>
        /// <param name="blobContainerName">Container name</param>
        /// <param name="eventId">Event id from log context</param>
        /// <returns>Blob client</returns>
        private static BlobClient GetBlobClient(BlobServiceClient blobServiceClient, string blobContainerName, string eventId)
        {
            var dt = DateTimeOffset.UtcNow;
            var containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            containerClient.CreateIfNotExists();

            // If event id is empty then create a new guid. This scenario is remotely likely to happen
            eventId = eventId ?? Guid.NewGuid().ToString();
            eventId = $"{dt:yyyy}{PathDivider}{dt:MM}{PathDivider}{dt:dd}{PathDivider}{dt:yyyyMMdd_HHMM}_{eventId}.log";
            return containerClient.GetBlobClient(eventId);
        }
    }
}
