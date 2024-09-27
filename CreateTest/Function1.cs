using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Data.Tables;
using CloudPOE2.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CreateTest
{
    public static class Function1
    {

        private static readonly string _connectionString = "DefaultEndpointsProtocol=https;AccountName=st10273388;AccountKey=1hz1HlWDQsoTpqqkOQ329MFGPZT5WPOE9w0xDXxqgCZmVAGpCkUcUZSy7wCwMbvz+DhXsO5qJtVu+AStfjE8uQ==;EndpointSuffix=core.windows.net";
        private static readonly string _tableName = "ProductsCloudPOE";


        [Function("CreateProducts")]
        public static async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function,"get", "post", Route = null)] HttpRequestData req,
            FunctionContext executionContext)
            {
                var logger = executionContext.GetLogger("CreateProducts");
                logger.LogInformation("Processing request...");

            // this code will be to request the body and deserialixe to the product object 
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var product = JsonConvert.DeserializeObject<Product>(requestBody);

            // this code will validate the product data 
            if (product == null || string.IsNullOrEmpty(product.Product_Id.ToString() ) )
            {

                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid product data.");
                return badResponse;

            }

            // Create TableServiceClient to connect to Azure Table Storage
            var serviceClient = new TableServiceClient(_connectionString);
            var tableClient = serviceClient.GetTableClient(_tableName);

            // Ensure the table exists
            await tableClient.CreateIfNotExistsAsync();

            // Set the PartitionKey and RowKey for the Product entity
            product.PartitionKey = product.Category ?? "DefaultCategory";  // Set Category as PartitionKey or default value
            product.RowKey = product.Product_Id.ToString(); // Set Product_Id as RowKey

            // Insert the Product entity into the table
            await tableClient.AddEntityAsync(product);

            // Return success response
            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteStringAsync("Product created successfully!");
            return response;
        }
    }
}
