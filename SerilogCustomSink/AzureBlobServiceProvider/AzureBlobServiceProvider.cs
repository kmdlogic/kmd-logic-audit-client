using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace Kmd.Logic.CustomSink.AzureBlobOrEventHub
{
    public class AzureBlobServiceProvider : IAzureBlobServiceProvider
    {
        public async Task UploadBlobAsync(BlobServiceClient blobServiceClient, string blobContainerName, string blobName)
        {
            // Create the container and return a container client object
            BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(blobContainerName);

        }

    }
}
