using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace CloudPOE2.Services
{
    // Service for working with Azure Blob Storage
    public class BlobService
    {
        // Client to interact with Azure Blob Storage
        private readonly BlobServiceClient _blobServiceClient;
        // Name of the container in Blob Storage
        private readonly string _containerName = "productscloudpoe";

        // Constructor to initialize the BlobServiceClient using a connection string
        public BlobService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        // Method to upload a file to the Blob Storage and return its URI
        public async Task<string> UploadAsync(Stream fileStream, string fileName)
        {
            // Get the container and blob clients
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            // Upload the file
            await blobClient.UploadAsync(fileStream);
            // Return the URI of the uploaded blob
            return blobClient.Uri.ToString();
        }

        // Method to delete a blob from Blob Storage using its URI
        public async Task DeleteBlobAsync(string blobUri)
        {
            // Extract the blob name from the URI
            Uri uri = new Uri(blobUri);
            string blobName = uri.Segments[^1];
            // Get the container and blob clients
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            // Delete the blob if it exists, including any snapshots
            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }
    }
}
