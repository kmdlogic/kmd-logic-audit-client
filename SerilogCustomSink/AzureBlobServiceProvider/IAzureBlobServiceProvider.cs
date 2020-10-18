using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace Kmd.Logic.CustomSink.AzureBlobOrEventHub
{
    public interface IAzureBlobServiceProvider
    {
        Task UploadBlobAsync(BlobServiceClient blobServiceClient, string blobContainerName, string blobName);
    }
}
