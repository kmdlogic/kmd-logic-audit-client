using Azure.Storage.Blobs;

namespace Kmd.Logic.Audit.Client.AzureBlobOrEventHubSink
{
    public interface IAzureBlobServiceProvider
    {
        string UploadBlob(BlobServiceClient blobServiceClient, string blobContainerName, string eventId, string content);
    }
}
