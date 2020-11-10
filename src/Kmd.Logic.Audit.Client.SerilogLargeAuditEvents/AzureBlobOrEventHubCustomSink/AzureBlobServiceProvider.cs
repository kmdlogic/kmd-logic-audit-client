using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;

namespace Kmd.Logic.Audit.Client.SerilogLargeAuditEvents.AzureBlobOrEventHubCustomSink
{
    public static class AzureBlobServiceProvider
    {
        public const string PathDivider = "/";

        /// <summary>
        /// Considering 10 MB of minimum size for each block
        /// </summary>
        private const int BlockSize = 10 * 1024 * 1024;

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
            var dt = DateTimeOffset.UtcNow;

            // If event id is empty then create a new guid. This scenario is remotely likely to happen
            eventId = eventId ?? Guid.NewGuid().ToString();

            // Form blob name
            var blobName = $"{dt:yyyy}{PathDivider}{dt:MM}{PathDivider}{dt:dd}{PathDivider}{dt:yyyyMMdd_HHMM}_{eventId}.log";

            // Get a reference to a blob
            var containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            var blockBlobClient = containerClient.GetBlockBlobClient(blobName);

            // Check if message size is greater than the minimum block size then divide the message into chunks
            if (Encoding.UTF8.GetByteCount(content) > BlockSize)
            {
                UploadBlobInChunk(blobServiceClient, blobContainerName, blobName, content);
                return blockBlobClient.Uri;
            }
            else
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    try
                    {
                        blockBlobClient.Upload(stream);
                        return blockBlobClient.Uri;
                    }
                    catch (Exception ex)
                    {
                        Serilog.Debugging.SelfLog.WriteLine($"Exception {ex} thrown while trying to upload blob.");
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// When blob size is more then upload blob in chunks
        /// </summary>
        /// <param name="blobServiceClient">The blob service client</param>
        /// <param name="blobContainerName">Container name</param>
        /// <param name="eventId">Event id</param>
        /// <param name="content">Blob content to be uploaded</param>
        private static void UploadBlobInChunk(BlobServiceClient blobServiceClient, string blobContainerName, string eventId, string content)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            var blockBlobClient = containerClient.GetBlockBlobClient(eventId);
            int offset = 0;
            int counter = 0;
            var blockIds = new List<string>();
            var totalBytes = Encoding.UTF8.GetByteCount(content);
            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    do
                    {
                        var dataToRead = Math.Min(totalBytes, BlockSize);
                        byte[] data = new byte[dataToRead];
                        var dataRead = stream.Read(data, offset, (int)dataToRead);
                        totalBytes -= dataRead;
                        if (dataRead > 0)
                        {
                            var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(counter.ToString("d3", CultureInfo.InvariantCulture)));
                            blockBlobClient.StageBlock(blockId, new MemoryStream(data));
                            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Block {0} uploaded successfully.", counter.ToString("d3", CultureInfo.InvariantCulture)));
                            blockIds.Add(blockId);
                            counter++;
                        }
                    }
                    while (totalBytes > 0);
                    blockBlobClient.CommitBlockList(blockIds);
                }
            }
            catch (Exception ex)
            {
                Serilog.Debugging.SelfLog.WriteLine($"Exception {ex} thrown while trying to upload blob in chunks.");
            }
        }
    }
}
