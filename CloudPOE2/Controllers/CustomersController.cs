﻿using CloudPOE2.Models;
using CloudPOE2.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudPOE2.Controllers
{
    public class CustomersController : Controller
    {

        private readonly TableStorageService _tableStorageService;

        public CustomersController(TableStorageService tableStorageService)
        {
            _tableStorageService = tableStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _tableStorageService.GetAllCustomersAsync();
            return View(products);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            customer.PartitionKey = "CustomersPartition";
            customer.RowKey = Guid.NewGuid().ToString();

            await _tableStorageService.AddCustomerAsync(customer);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            await _tableStorageService.DeleteCustomerAsync(partitionKey, rowKey);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Details(string partitionKey, string rowKey)
        {
            var customer = await _tableStorageService.GetCustomerAsync(partitionKey, rowKey);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }
    }
}
