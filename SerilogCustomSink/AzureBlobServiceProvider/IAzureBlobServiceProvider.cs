using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace Kmd.Logic.CustomSink.AzureBlobOrEventHub
{
    public interface IAzureBlobServiceProvider
    {
        void UploadBlob(BlobServiceClient blobServiceClient, string blobContainerName, string blobName, IEnumerable<string> content);
    }
}
