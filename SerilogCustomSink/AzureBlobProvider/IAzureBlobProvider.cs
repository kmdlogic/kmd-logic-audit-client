using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;

namespace Kmd.Logic.CustomSink.AzureBlobOrEventHub
{
    public interface IAzureBlobProvider
    {
        Task UploadBlobAsync(CloudBlobClient cloudBlobClient, string blobContainerName, string blobName);
    }
}
