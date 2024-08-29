// Using statements to include necessary namespaces
using CloudPOE2.Models;
using CloudPOE2.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudPOE2.Controllers
{
    public class FilesController : Controller
    {
        // This brings in the Azure File Share service
        private readonly AzureFileShareService _fileShareService;

        // Constructor that initializes the Azure File Share service
        public FilesController(AzureFileShareService fileShareService)
        {
            _fileShareService = fileShareService;
        }

        // This is a Action method to display the list of files in the "uploads" directory
        public async Task<IActionResult> Index()
        {
            List<FileModel> files;
            try
            {
                // Get the retrieve the list of files from Azure File Share
                files = await _fileShareService.ListFilesAsync("uploads");
            }
            catch (Exception ex)
            {
                // If an error occurs, display a message and initialize an empty file list
                ViewBag.Message = $"Failed to load files: {ex.Message}";
                files = new List<FileModel>();
            }

            // Return the view with the list of files
            return View(files);
        }

        // Action method to handle file upload
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // Check if a file was selected  
            if (file == null || file.Length == 0)
            {
                // Add an error to the model state if no file was selected
                ModelState.AddModelError("file", "Please select a file to upload.");
                return await Index(); // Return to the Index view with the error
            }

            try

            {
                // Open a stream to read the file content
                using (var stream = file.OpenReadStream())
                {
                    string directoryName = "uploads"; // Directory where the file will be uploaded
                    string fileName = file.FileName;   // Name of the file

                    // Upload the file to Azure File Share
                    await _fileShareService.UploadFileAsync(directoryName, fileName, stream);
                }

                // If upload is successful, display a success message
                TempData["Message"] = $"File '{file.FileName}' uploaded successfully!";
            }
            catch (Exception ex)
            {
                // If upload fails, display an error message
                TempData["Message"] = $"File upload failed: {ex.Message}";
            }

            // Redirect back to the Index action method
            return RedirectToAction("Index");
        }

        // Action method to handle file download
        [HttpGet]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            // Check if the file name is provided
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File name cannot be null or empty.");
            }

            try
            {
                // Attempt to download the file from Azure File Share
                var fileStream = await _fileShareService.DownloadFileAsync("uploads", fileName);

                // If the file is not found, return a NotFound result
                if (fileStream == null)
                {
                    return NotFound($"File '{fileName}' not found.");
                }

                // Return the file to the client as a download
                return File(fileStream, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                // If an error occurs during download, return a BadRequest result
                return BadRequest($"Error downloading file: {ex.Message}");
            }
        }
    }
}
