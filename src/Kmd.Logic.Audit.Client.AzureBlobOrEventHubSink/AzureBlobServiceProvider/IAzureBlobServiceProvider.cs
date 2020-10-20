using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    public interface IAzureBlobServiceProvider
    {
        string UploadBlob(BlobServiceClient blobServiceClient, string blobContainerName, string blobName, string content);
    }
}
