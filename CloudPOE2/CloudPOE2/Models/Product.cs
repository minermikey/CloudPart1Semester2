using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace CloudPOE2.Models
{
    public class Product : ITableEntity
    {

        // This class is extremely important for ensuring that a products information is correctely saved lets run through it 


        [Key]
        public int Product_Id { get; set; }  // Ensure this property exists and is populated
        public string? Product_Name { get; set; }  // Ensure this property exists and is populated
        public string? Description { get; set; } // Any informatin that might need to be added 
        public string? ImageUrl { get; set; } // where is the image 
        public string? Category { get; set; } // Which does it belong too 
        public int? Price { get; set; } // literally the price how much simplier 
 

        // ITableEntity implementation
        // dont reall wanna mess wiht these bits of informaitno, its super important and can mess up you code 
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; } // this and partition key is basically like your primary key for the table 
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

    }
}
