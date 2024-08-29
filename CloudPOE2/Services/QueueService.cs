using Azure.Storage.Queues;

namespace CloudPOE2.Services
{
    // Service for working with Azure Queue Storage
    public class QueueService
    {
        // hi Mick 
        // Client to interact with the Azure Queue
        private readonly QueueClient _queueClient;

        // Constructor to initialize the QueueClient using a connection string and queue name
        public QueueService(string connectionString, string queueName)
        {
            _queueClient = new QueueClient(connectionString, queueName);
        }

        // Method to send a message to the Azure Queue
        public async Task SendMessageAsync(string message)
        {
            await _queueClient.SendMessageAsync(message);
        }
    }
}
