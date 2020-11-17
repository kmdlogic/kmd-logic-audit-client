using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;

namespace Kmd.Logic.Audit.Client.SerilogLargeAuditEvents.AzureBlobOrEventHubCustomSink
{
    public static class AzureBlobUploadManager
    {
        public const string PathDivider = "/";

        /// <summary>
        /// Considering 4 MB of minimum size for each block
        /// </summary>
        private const int BlockSize = 4 * 1000 * 1000;

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
            eventId = eventId ?? Guid.NewGuid().ToString();
            var auditEventSize = Encoding.UTF8.GetByteCount(content);
            var blobName = $"{dt:yyyy}{PathDivider}{dt:MM}{PathDivider}{dt:dd}{PathDivider}{dt:yyyyMMdd_HHMM}_{eventId}.log";

            // Get a reference to a blob
            var containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            containerClient.CreateIfNotExists();
            var blockBlobClient = containerClient.GetBlockBlobClient(blobName);
            if (auditEventSize > BlockSize)
            {
                Serilog.Debugging.SelfLog.WriteLine($"Audit event with event id {eventId} of size {auditEventSize} bytes exceeding threshold, hence uploading blob in chunks.");
                UploadBlobInChunk(blockBlobClient, content);
                return blockBlobClient.Uri;
            }
            else
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    blockBlobClient.Upload(stream);
                    return blockBlobClient.Uri;
                }
            }
        }

        /// <summary>
        /// When blob size is more then upload blob in chunks
        /// </summary>
        /// <param name="blockBlobClient">The blob service client</param>
        /// <param name="content">Blob content to be uploaded</param>
        private static void UploadBlobInChunk(BlockBlobClient blockBlobClient, string content)
        {
            int offset = 0;
            int counter = 0;
            var blockNumbers = new List<string>();
            var totalBytes = Encoding.UTF8.GetByteCount(content);
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
                        var blockNumber = Convert.ToBase64String(Encoding.UTF8.GetBytes(counter.ToString("d3", CultureInfo.InvariantCulture)));
                        blockBlobClient.StageBlock(blockNumber, new MemoryStream(data));
                        blockNumbers.Add(blockNumber);
                        counter++;
                    }
                }
                while (totalBytes > 0);
                blockBlobClient.CommitBlockList(blockNumbers);
            }
        }
    }
}
