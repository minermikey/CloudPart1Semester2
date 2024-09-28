using System.Net;
using System.Text.Json;
using Azure;
using CloudPOE2.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using CloudPOE2.Services;

namespace CloudPOE2.Functions
{
    public class ProductFunction
    {
        private readonly TableStorageService _tableStorageService;

        // Constructor to initialize blob and table storage services
        public ProductFunction(TableStorageService tableStorageService)
        {
            _tableStorageService = tableStorageService;
        }

        [Function("AddProductVersion2")]
        public async Task<HttpResponseData> AddProductAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestData req,
            FunctionContext context)
        {
            var logger = context.GetLogger("ProductFunction");
            logger.LogInformation("Processing AddProduct request.");

            // Read the body content as a stream
            var body = await req.ReadAsStringAsync();

            // Deserialize the JSON body into a Product object
            var product = JsonSerializer.Deserialize<Product>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (product == null)
            {
                logger.LogError("Invalid product data.");
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                errorResponse.WriteString("Invalid product data.");
                return errorResponse;
            }

            //// Check if the request contains any file data
            //var file = req.Body;
            //if (file != null)
            //{
            //    using var stream = new MemoryStream();
            //    await file.CopyToAsync(stream);
            //    stream.Position = 0;

            //    // Upload the file to blob storage
            //    var imageUrl = await _blobService.UploadAsync(stream, "uploaded-file-name"); // Change the name as needed
            //    product.ImageUrl = imageUrl;
            //}

            // Set partition and row keys for table storage
            product.PartitionKey = "ProductsPartition";
            product.RowKey = Guid.NewGuid().ToString();

            // Add the product to table storage
            await _tableStorageService.AddProductAsync(product);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync("Product added successfully.");
            return response;
        }

        //[Function("DeleteProduct")]
        //public async Task<HttpResponseData> DeleteProductAsync(
        //    [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "products/{partitionKey}/{rowKey}")] HttpRequestData req,
        //    string partitionKey,
        //    string rowKey,
        //    FunctionContext context)
        //{
        //    var logger = context.GetLogger("ProductFunction");
        //    logger.LogInformation($"Deleting product with PartitionKey: {partitionKey}, RowKey: {rowKey}.");

        //    // Retrieve the product by partitionKey and rowKey
        //    var product = await _tableStorageService.GetProductAsync(partitionKey, rowKey);
        //    if (product == null)
        //    {
        //        var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
        //        notFoundResponse.WriteString("Product not found.");
        //        return notFoundResponse;
        //    }

        //    // Delete the product image from blob storage if it exists
        //    if (!string.IsNullOrEmpty(product.ImageUrl))
        //    {
        //        await _blobService.DeleteBlobAsync(product.ImageUrl);
        //    }

        //    // Delete the product from table storage
        //    await _tableStorageService.DeleteProductAsync(partitionKey, rowKey);

        //    var response = req.CreateResponse(HttpStatusCode.OK);
        //    await response.WriteStringAsync("Product deleted successfully.");
        //    return response;
        //}

        //[Function("GetAllProducts")]
        //public async Task<HttpResponseData> GetAllProductsAsync(
        //    [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestData req,
        //    FunctionContext context)
        //{
        //    var logger = context.GetLogger("ProductFunction");
        //    logger.LogInformation("Retrieving all products.");

        //    // Retrieve all products from table storage
        //    var products = await _tableStorageService.GetAllProductsAsync();

        //    var response = req.CreateResponse(HttpStatusCode.OK);
        //    await response.WriteAsJsonAsync(products);
        //    return response;
        //}
    }
}
