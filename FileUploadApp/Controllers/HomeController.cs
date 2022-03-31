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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using FileUploadApp.Models;


namespace FileUploadApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment env;
        private readonly MyDbContext dbContext;

        // name of our upload folder
        private const string UPLOAD_DIR = "uploads";

        // dependency-injecting both web environment and database context
        // into our constructor
        public HomeController(IWebHostEnvironment env, MyDbContext dbContext)
        {
            this.env = env;
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            Session session = ValidateSession();
            if (session == null)
            {
                // no session; bring user to Login page
                return RedirectToAction("Index", "Login");
            }

            // get all photos associated with this user
            List<Photo> photos = dbContext.Photos.Where(x => 
                                    x.User == session.User).ToList();
            
            // pass data to View via the ViewBag object
            ViewBag.photos = photos;
            ViewBag.uploadDir = "../" + UPLOAD_DIR;            

            // return composed View to user
            return View();
        }

        public IActionResult UploadFile(IFormFile myfile)
        {            
            Session session = ValidateSession();
            if (session == null)
            {
                // no session; bring user to Login page
                return RedirectToAction("Index", "Login");
            }

            string filename = SaveFile(myfile);     // save file                        
            MapUserToPhoto(session.User, filename); // update database

            // showing Index view again, this time the newly-uploaded photo
            // will also be included in the View.
            return RedirectToAction("Index", "Home");
        }

        public IActionResult DeletePhoto([FromBody] ClientReq req)
        {
            Session session = ValidateSession();
            
            if (session != null && req.PhotoId != null)
            {
                // query database based on primary key of photo
                Photo photo = dbContext.Photos.FirstOrDefault(x => 
                    x.Id.ToString() == req.PhotoId);
                
                if (photo != null)  // found the record in our database
                {
                    if (DeleteFile(photo.Filename))
                    {
                        // remove the database record
                        dbContext.Remove(photo);

                        // must remember to commit for changes to take place
                        dbContext.SaveChanges();

                        // informs user that delete is successful
                        return Json(new { status = "success" });
                    }
                }
            }

            return Json(new { status = "fail" });
        }

        // Resize and save the uploaded file into the filesystem
        private string SaveFile(IFormFile myfile) 
        {                                   
            if (myfile != null)
            {
                // create a "uploads" directory if it does not exist
                // the path should be "wwwroot/uploads"            
                string path = Path.Combine(env.WebRootPath, UPLOAD_DIR);
                if (! Directory.Exists(path)) 
                {
                    Directory.CreateDirectory(path);
                }   

                // construct "non-clashable" filename
                string filename = Path.GetFileName(myfile.FileName);                
                string ext = Path.GetExtension(filename);   // ext includes the "."
                
                string randName = Guid.NewGuid().ToString();
                string newFilename = String.Format("{0}{1}", randName, ext);

                // resize image using 3rd-party library - ImageSharp
                using (Image image = Image.Load(myfile.OpenReadStream())) 
                {
                    int newWidth = 300;
                    int newHeight = 300;

                    if (image.Width > image.Height)
                    {
                        newHeight = (newWidth * image.Height) / image.Width;
                    }                        
                    else 
                    {
                        newWidth = (newHeight * image.Width) / image.Height;
                    }                        

                    image.Mutate(x => x.Resize(newWidth, newHeight));
                    image.Save(String.Format("{0}/{1}", path, newFilename));
                }

                return newFilename;
            }

            return null;
        }

        // Update the database to associate a user with the uploaded file
        private void MapUserToPhoto(User user, string filename)
        {
            // create a new Photo model (this will be our new database record)
            Photo photo = new Photo();
            photo.User = user;
            photo.Filename = filename;

            // add record to database
            dbContext.Add(photo);       
            
            // commit; without this, entry will not be saved
            dbContext.SaveChanges();    
        }

        private bool DeleteFile(string filename)
        {
            string path = Path.Combine(env.WebRootPath, UPLOAD_DIR);
            string target = String.Format("{0}/{1}", path, filename);
            
            if (System.IO.File.Exists(target))
            {
                System.IO.File.Delete(target);
                return true;
            }

            return false;
        }

        private Session ValidateSession()
        {
            // check if there is a SessionId cookie
            if (Request.Cookies["SessionId"] == null)
            {
                return null;
            }

            // convert into a Guid type (from a string type)
            Guid sessionId = Guid.Parse(Request.Cookies["SessionId"]);
            Session session = dbContext.Sessions.FirstOrDefault(x =>
                x.Id == sessionId
            );

            return session;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
