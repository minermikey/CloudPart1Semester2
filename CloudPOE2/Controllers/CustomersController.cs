using CloudPOE2.Models;
using CloudPOE2.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudPOE2.Controllers
{
    public class CustomersController : Controller
    {

        // creating a reffrence of the table storage 
        private readonly TableStorageService _tableStorageService;

        // adding the class to this class 
        public CustomersController(TableStorageService tableStorageService)
        {
            _tableStorageService = tableStorageService;
        }


        // the index view controller, it stores the information from the get customer async and then displays it 
        public async Task<IActionResult> Index()
        {
            // storing the information in the of the custoemers in products 
            var products = await _tableStorageService.GetAllCustomersAsync();
            return View(products);
        }

        // returning the create view it just returns the view 
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        // returning the create async function
        public async Task<IActionResult> Create(Customer customer)
        {
            // this set the partition key 
            customer.PartitionKey = "CustomersPartition";
            // this generates a unqiue key 
            customer.RowKey = Guid.NewGuid().ToString();
            // adds the information that 
            await _tableStorageService.AddCustomerAsync(customer);
            return RedirectToAction("Index");
        }

        // this is the delete fuunction 
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            // deletes the partition key and row key 
            await _tableStorageService.DeleteCustomerAsync(partitionKey, rowKey);
            return RedirectToAction("Index");
        }


        // this return the details view 
        public async Task<IActionResult> Details(string partitionKey, string rowKey)
        {
            // same story 
            var customer = await _tableStorageService.GetCustomerAsync(partitionKey, rowKey);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }
    }
}
