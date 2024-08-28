using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace CloudPOE2.Models
{
    public class Order : ITableEntity
    {
        [Key]
        public int Order_Id { get; set; }

        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        [Required(ErrorMessage = "Please select a customer.")]
        public int Customer_ID { get; set; } // FK to the Customer who made the order

        [Required(ErrorMessage = "Please select a product.")]
        public int Product_ID { get; set; } // FK to the Product being ordered

        [Required(ErrorMessage = "Please select the order date.")]
        public DateTime Order_Date { get; set; }

        // Add other properties as needed, such as order status, total cost, etc.
    }
}