using CloudPOE2.Models;
using CloudPOE2.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudPOE2.Controllers
{
    public class ProductsController : Controller
    {
        // Services for handling blobs and table storage
        private readonly BlobService _blobService;
        private readonly TableStorageService _tableStorageService;

        // Constructor to initialize the services
        public ProductsController(BlobService blobService, TableStorageService tableStorageService)
        {
            _blobService = blobService;
            _tableStorageService = tableStorageService;
        }

        // Display the form to add a product
        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        // Handle the form submission for adding a product
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product, IFormFile file)
        {
            if (file != null)
            {
                // Upload the file and get the image URL
                using var stream = file.OpenReadStream();
                var imageUrl = await _blobService.UploadAsync(stream, file.FileName);
                product.ImageUrl = imageUrl;
            }

            if (ModelState.IsValid)
            {
                // Set partition and row keys, then add the product to table storage
                product.PartitionKey = "ProductsPartition";
                product.RowKey = Guid.NewGuid().ToString();
                await _tableStorageService.AddProductAsync(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // Handle the request to delete a product
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(string partitionKey, string rowKey, Product product)
        {
            if (product != null && !string.IsNullOrEmpty(product.ImageUrl))
            {
                // Delete the associated image from blob storage
                await _blobService.DeleteBlobAsync(product.ImageUrl);
            }
            // Delete the product from table storage
            await _tableStorageService.DeleteProductAsync(partitionKey, rowKey);

            return RedirectToAction("Index");
        }

        // Display all products
        public async Task<IActionResult> Index()
        {
            var products = await _tableStorageService.GetAllProductsAsync();
            return View(products);
        }
    }
}
