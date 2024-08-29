using CloudPOE2.Models;
using CloudPOE2.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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

        // Show all orders
        public async Task<IActionResult> Index()
        {
            var orders = await _tableStorageService.GetAllOrdersAsync();
            return View(orders);
        }

        // Show the registration form
        public async Task<IActionResult> Register()
        {
            var customers = await _tableStorageService.GetAllCustomersAsync();
            var products = await _tableStorageService.GetAllProductsAsync();

            if (customers == null || customers.Count == 0)
            {
                ModelState.AddModelError("", "No customers found. Please add customers first.");
                return View();
            }

            if (products == null || products.Count == 0)
            {
                ModelState.AddModelError("", "No products found. Please add products first.");
                return View();
            }

            ViewData["Customers"] = customers;
            ViewData["Products"] = products;

            return View();
        }

        // Handle the registration form submission
        [HttpPost]
        public async Task<IActionResult> Register(Order order)
        {
            if (ModelState.IsValid)
            {
                order.Order_Date = DateTime.SpecifyKind(order.Order_Date, DateTimeKind.Utc);

                // Set PartitionKey and RowKey
                order.PartitionKey = "OrdersPartition";
                order.RowKey = Guid.NewGuid().ToString(); // Unique RowKey

                await _tableStorageService.AddOrderAsync(order);

                // Send a message to the queue about the new order
                string message = $"New Order by Customer {order.Customer_ID} for Product {order.Product_ID} on {order.Order_Date}";
                await _queueService.SendMessageAsync(message);

                return RedirectToAction("Index");
            }

            // If ModelState is invalid, reload customers and products lists
            var customers = await _tableStorageService.GetAllCustomersAsync();
            var products = await _tableStorageService.GetAllProductsAsync();
            ViewData["Customers"] = customers;
            ViewData["Products"] = products;

            return View(order);
        }
    }
}
