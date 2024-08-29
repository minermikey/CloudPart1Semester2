using Azure;
using Azure.Data.Tables;
using CloudPOE2.Models;

namespace CloudPOE2.Services
{
    // Service for interacting with Azure Table Storage
    public class TableStorageService
    {
        // Table clients for Products, Customers, and Orders tables
        private readonly TableClient _tableClient;
        private readonly TableClient _customerTableClient;
        private readonly TableClient _orderTableClient;

        // Constructor to initialize TableClients for different tables
        public TableStorageService(string connectionString)
        {
            _tableClient = new TableClient(connectionString, "ProductsCloudPOE");
            _customerTableClient = new TableClient(connectionString, "CustomersCloudPOE");
            _orderTableClient = new TableClient(connectionString, "OrdersCloudPOE");
        }

        // Get all products from the Products table
        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = new List<Product>();

            await foreach (var product in _tableClient.QueryAsync<Product>())
            {
                products.Add(product);
            }

            return products;
        }

        // Add a new product to the Products table
        public async Task AddProductAsync(Product product)
        {
            // Check that PartitionKey and RowKey are set
            if (string.IsNullOrEmpty(product.PartitionKey) || string.IsNullOrEmpty(product.RowKey))
            {
                throw new ArgumentException("PartitionKey and RowKey must be set.");
            }

            try
            {
                await _tableClient.AddEntityAsync(product);
            }
            catch (RequestFailedException ex)
            {
                // Handle error
                throw new InvalidOperationException("Error adding entity to Table Storage", ex);
            }
        }

        // Delete a product from the Products table
        public async Task DeleteProductAsync(string partitionKey, string rowKey)
        {
            await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        // Get a specific product from the Products table
        public async Task<Product?> GetProductAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<Product>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                // Handle not found
                return null;
            }
        }

        // Get all customers from the Customers table
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var customers = new List<Customer>();

            await foreach (var customer in _customerTableClient.QueryAsync<Customer>())
            {
                customers.Add(customer);
            }

            return customers;
        }

        // Add a new customer to the Customers table
        public async Task AddCustomerAsync(Customer customer)
        {
            // Check that PartitionKey and RowKey are set
            if (string.IsNullOrEmpty(customer.PartitionKey) || string.IsNullOrEmpty(customer.RowKey))
            {
                throw new ArgumentException("PartitionKey and RowKey must be set.");
            }

            try
            {
                await _customerTableClient.AddEntityAsync(customer);
            }
            catch (RequestFailedException ex)
            {
                // Handle error
                throw new InvalidOperationException("Error adding entity to Table Storage", ex);
            }
        }

        // Delete a customer from the Customers table
        public async Task DeleteCustomerAsync(string partitionKey, string rowKey)
        {
            await _customerTableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        // Get a specific customer from the Customers table
        public async Task<Customer?> GetCustomerAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await _customerTableClient.GetEntityAsync<Customer>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
                // Handle not found
                return null;
            }
        }

        // Add a new order to the Orders table
        public async Task AddOrderAsync(Order order)
        {
            // Check that PartitionKey and RowKey are set
            if (string.IsNullOrEmpty(order.PartitionKey) || string.IsNullOrEmpty(order.RowKey))
            {
                throw new ArgumentException("PartitionKey and RowKey must be set.");
            }

            try
            {
                await _orderTableClient.AddEntityAsync(order);
            }
            catch (RequestFailedException ex)
            {
                // Handle error
                throw new InvalidOperationException("Error adding order to Table Storage", ex);
            }
        }

        // Get all orders from the Orders table
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = new List<Order>();

            await foreach (var order in _orderTableClient.QueryAsync<Order>())
            {
                orders.Add(order);
            }

            return orders;
        }
    }
}
