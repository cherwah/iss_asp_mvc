using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using FileUploadApp.Models;


namespace FileUploadApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment env;
        private readonly MyDbContext dbContext;

        // dependency-injecting both web environment and database context
        // into our constructor
        public HomeController(IWebHostEnvironment env, MyDbContext dbContext)
        {
            this.env = env;
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UploadFile(IFormFile myfile)
        {
            SaveFile(myfile);

            return View("Index");
        }

        private void SaveFile(IFormFile myfile) 
        {           
            if (myfile != null)
            {
                string path = Path.Combine(env.WebRootPath, "uploads");
                if (! Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filename = Path.GetFileName(myfile.FileName);
                using (FileStream stream = new FileStream(
                    Path.Combine(path, filename), FileMode.Create))
                {
                    myfile.CopyTo(stream);
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
