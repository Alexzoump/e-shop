using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Frontend", "index.html"); // Αρχική σελίδα
            return PhysicalFile(filePath, "text/html");
        }
    }
}