using CloudPOE2.Models;
using CloudPOE2.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudPOE2.Controllers
{
    public class OrderController : Controller
    {

        private readonly TableStorageService _tableStorageService;
        private readonly QueueService _queueService;

        public OrderController(TableStorageService tableStorageService, QueueService queueService)
        {
            _tableStorageService = tableStorageService;
            _queueService = queueService;
        }



        // Action to display all sightings (optional first)
        public async Task<IActionResult> Index()
        {
            var orders = await _tableStorageService.GetAllOrdersAsync();
            return View(orders);
        }


        public async Task<IActionResult> Register()
        {
            var customers = await _tableStorageService.GetAllCustomersAsync();
            var products = await _tableStorageService.GetAllProductsAsync();

            // Check for null or empty lists
            if (customers == null || customers.Count == 0)
            {
                // Handle the case where no birders are found
                ModelState.AddModelError("", "No birders found. Please add birders first.");
                return View(); // Or redirect to another action
            }

            if (products == null || products.Count == 0)
            {
                // Handle the case where no birds are found
                ModelState.AddModelError("", "No birds found. Please add birds first.");
                return View(); // Or redirect to another action
            }

            ViewData["Birders"] = customers;
            ViewData["Birds"] = products;

            return View();
        }



        // Action to handle the form submission and register the sighting
        [HttpPost]
        public async Task<IActionResult> Register(Order order)
        {
            if (ModelState.IsValid)
            {//TableService
                order.Order_Date = DateTime.SpecifyKind(order.Order_Date, DateTimeKind.Utc);
                order.PartitionKey = "OrdersPartition";
                order.RowKey = Guid.NewGuid().ToString();
                await _tableStorageService.AddOrderAsync(order);
                //MessageQueue
                string message = $"New Order by Customer {order.Customer_ID} for {order.Product_ID} on {order.Order_Date}";
                await _queueService.SendMessageAsync(message);

                return RedirectToAction("Index");
            }
            else
            {
                // Log model state errors
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }

            // Reload birders and birds lists if validation fails
            var customers = await _tableStorageService.GetAllCustomersAsync();
            var products = await _tableStorageService.GetAllProductsAsync();
            ViewData["Customers"] = customers;
            ViewData["Products"] = products;

            return View(order);
        }






    }
}
