using CloudPOE2.Models;
using CloudPOE2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace CloudPOE2.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _repository;

        public AccountController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("AzureStorage");
            string tableName = configuration["Users"];

            _repository = new UserRepository(connectionString, tableName);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Generate a salt
                string salt = Guid.NewGuid().ToString();

                // Hash the password with the salt
                string hashedPassword = HashPassword(model.Password, salt);

                User user = new User
                {
                    PartitionKey = model.Username,  // Typically, PartitionKey is used for grouping similar entities
                    RowKey = model.Username,        // RowKey uniquely identifies the entity within the Partition
                    Email = model.Email,
                    Password = hashedPassword,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Salt = salt // Store the salt for later password verification
                };

                await _repository.CreateUserAsync(user);

                // Redirect to login page after successful registration
                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the user by email
                User user = await _repository.GetUserAsync(model.Username);
                // && user.Password == HashPassword(model.Password, user.Salt)


                // works 
                // User != null &&
                //  == HashPassword(model.Password, user.Salt)
                if (user.Password.Equals(HashPassword(model.Password, user.Salt)))
                {
                    // Store user details in session or handle RememberMe functionality
                    //HttpContext.Session.SetString("Username", user.Username ?? user.PartitionKey);
                    //HttpContext.Session.SetString("Email", user.Email);
                    //HttpContext.Session.SetString("FirstName", user.FirstName);
                    //HttpContext.Session.SetString("LastName", user.LastName);


                    // Redirect to dashboard after successful login
                     return RedirectToAction("Index","Home");
                }
                else 
                {

                    await Console.Out.WriteLineAsync("Did not read");

                }

                // Display error message if login fails
                ModelState.AddModelError("", "Invalid email or password");
            }

            return View(model);
        }


        private string HashPassword(string password, string salt)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var saltBytes = Encoding.UTF8.GetBytes(salt);
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];
                Array.Copy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
                Array.Copy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);

                var hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
