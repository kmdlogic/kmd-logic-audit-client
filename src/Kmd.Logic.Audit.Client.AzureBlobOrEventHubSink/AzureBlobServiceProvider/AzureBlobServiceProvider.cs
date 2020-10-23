using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    public class AzureBlobServiceProvider : IAzureBlobServiceProvider
    {
        /// <summary>
        /// Uploads blob and return the blob url
        /// </summary>
        /// <param name="blobServiceClient">Blob service client</param>
        /// <param name="blobContainerName">Container name</param>
        /// <param name="blobName">Blob name</param>
        /// <param name="content">Content to be uploaded</param>
        /// <returns>Blob url</returns>
        public string UploadBlob(BlobServiceClient blobServiceClient, string blobContainerName, string blobName, string content)
        {
            // Get a reference to a blob
            BlobClient blobClient = this.GetBlobClient(blobServiceClient, blobContainerName, blobName);
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                try
                {
                    blobClient.Upload(stream);
                    var blobUrl = blobClient.Uri.ToString();
                    return blobUrl;
                }
                catch (Exception ex)
                {
                    Serilog.Debugging.SelfLog.WriteLine($"Exception {ex} thrown while trying to upload blob.");
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Check if container or blob exists or not and return a blob client object accordingly
        /// </summary>
        /// <param name="blobServiceClient">Blob service client</param>
        /// <param name="blobContainerName">Container name</param>
        /// <param name="blobName">Blob name</param>
        /// <returns>Blob client</returns>
        private BlobClient GetBlobClient(BlobServiceClient blobServiceClient, string blobContainerName, string blobName)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            var containerExistsClient = containerClient.Exists();
            if (!containerExistsClient.Value)
            {
                containerClient = blobServiceClient.CreateBlobContainer(blobContainerName);
            }

            // Check if blob with same name already exists, if exists then append the UTC DateTime to the blob name to avoid exception
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            var blobExistsClient = blobClient.Exists();
            if (blobExistsClient.Value)
            {
#pragma warning disable CA1305 // Specify IFormatProvider
                blobName = blobName + DateTime.Now.ToString();
#pragma warning restore CA1305 // Specify IFormatProvider
            }

            return containerClient.GetBlobClient(blobName);
        }
    }
}
