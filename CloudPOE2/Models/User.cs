using Microsoft.WindowsAzure.Storage.Table;

namespace CloudPOE2.Models
{
    public class User : TableEntity
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Salt { get; set; } // Add this property to store the salt
    }
}
