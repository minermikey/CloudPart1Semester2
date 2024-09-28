using System.Net;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BlobServicesAHhhhhh
{
    public class Function1
    {
        private static readonly string _connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private static readonly string _containerName = Environment.GetEnvironmentVariable("BlobContainerName");
        private static readonly BlobServiceClient _blobServiceClient = new BlobServiceClient(_connectionString);

        [FunctionName("UploadBlob")]
        public async Task<string> UploadAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                // Check if the request contains files
                if (req.Form.Files.Count == 0)
                {
                    log.LogWarning("No files were uploaded.");
                    return "No files were uploaded.";
                }

                var file = req.Form.Files[0];

                // Validate the file
                if (file.Length > 0)
                {
                    using (var fileStream = file.OpenReadStream())
                    {
                        // Sanitize the file name and create a unique name
                        var sanitizedFileName = Path.GetFileNameWithoutExtension(file.FileName);
                        var fileExtension = Path.GetExtension(file.FileName);
                        var uniqueFileName = $"{sanitizedFileName}_{Guid.NewGuid()}{fileExtension}";

                        // Get the container client
                        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                        await containerClient.CreateIfNotExistsAsync(); // Ensure the container exists

                        // Get the blob client
                        var blobClient = containerClient.GetBlobClient(uniqueFileName);

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

                log.LogWarning("Uploaded file is empty.");
                return "Uploaded file is empty.";
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error occurred while uploading the blob.");
                return "File upload failed.";
            }
        }
    }
}
