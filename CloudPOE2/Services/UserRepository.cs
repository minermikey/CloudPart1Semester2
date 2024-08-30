using CloudPOE2.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CloudPOE2.Services
{
    public class UserRepository
    {
        private readonly CloudTable _table;

        public UserRepository(string connectionString, string tableName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference("Users");

            // Ensure the table exists
            _table.CreateIfNotExistsAsync().Wait();
        }

        public async Task CreateUserAsync(User user)
        {
            // Hash the password
            user.Password = HashPassword(user.Password);

            // Create a new table operation
            TableOperation insertOperation = TableOperation.Insert(user);

            // Execute the operation
            await _table.ExecuteAsync(insertOperation);
        }

        public async Task<User> GetUserAsync(string username)
        {
            // Create a table query to fetch user by PartitionKey
            TableOperation retrieveOperation = TableOperation.Retrieve<User>(username, username);
            TableResult result = await _table.ExecuteAsync(retrieveOperation);

            return result.Result as User;
        }

        private string HashPassword(string password)
        {
            // Implement your password hashing algorithm here
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}
