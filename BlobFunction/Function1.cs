using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace CloudPOE2.Functions
{
    public class BlobFunction
    {
        private static readonly string _connectionString = "DefaultEndpointsProtocol=https;AccountName=st10273388;AccountKey=1hz1HlWDQsoTpqqkOQ329MFGPZT5WPOE9w0xDXxqgCZmVAGpCkUcUZSy7wCwMbvz+DhXsO5qJtVu+AStfjE8uQ==;EndpointSuffix=core.windows.net";
        private static readonly string _containerName = "productscloudpoe"; // Declare your container name
        private static readonly BlobServiceClient _blobServiceClient = new BlobServiceClient(_connectionString);

        [FunctionName("UploadBlob")]
        public async Task<string> UploadAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Read the file from the request
            var file = req.Form.Files[0]; // Adjust this according to your requirements
            if (file.Length > 0)
            {
                using (var fileStream = file.OpenReadStream())
                {
                    var fileName = file.FileName; // You might want to sanitize or modify this

                    // Get the container client
                    var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                    await containerClient.CreateIfNotExistsAsync(); // Ensure the container exists

                    // Get the blob client
                    var blobClient = containerClient.GetBlobClient(fileName);

                    // Set the access tier when uploading a blob
                    var uploadOptions = new BlobUploadOptions
                    {
                        AccessTier = AccessTier.Cool
                    };

                    // Upload the file
                    await blobClient.UploadAsync(fileStream, uploadOptions);

                    // Return the URI of the uploaded blob
                    return blobClient.Uri.ToString();
                }
            }

            return "File upload failed.";
        }
    }
}
