using CloudPOE2.Models;
using CloudPOE2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Required for session management
using Microsoft.Extensions.Configuration; // Required for IConfiguration
using System;
using System.Text;
using System.Threading.Tasks;

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
                // Retrieve the user by username
                User user = await _repository.GetUserAsync(model.Username);

                if (user != null && user.Password == HashPassword(model.Password, user.Salt))
                {
                    // Store user details in session
                    HttpContext.Session.SetString("Username", user.PartitionKey);
                    HttpContext.Session.SetString("Email", user.Email);
                    HttpContext.Session.SetString("FirstName", user.FirstName);
                    HttpContext.Session.SetString("LastName", user.LastName);

                    // Redirect to dashboard after successful login
                    return RedirectToAction("Dashboard", "Home");
                }

                // Display error message if login fails
                ModelState.AddModelError("", "Invalid username or password");
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
