using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using FileUploadApp.Models;

namespace FileUploadApp.Controllers
{
    public class LoginController : Controller
    {
      private MyDbContext dbContext;

      public LoginController(MyDbContext dbContext)
      {
          this.dbContext = dbContext;
      }

      public IActionResult Index()
      {
        if (Request.Cookies["SessionId"] != null) {
            Guid sessionId = Guid.Parse(Request.Cookies["sessionId"]);
            Session session = dbContext.Sessions.FirstOrDefault(x =>
                x.Id == sessionId
            );

            if (session == null) {
                // someone has used an invalid Session ID (to fool us?); 
                // route to Logout controller
                return RedirectToAction("Index", "Logout");
            }

            // valid Session ID; route to Home page
            return RedirectToAction("Index", "Home");
        }

        // no Session ID; show Login page
        return View();
      }

      public IActionResult Login(IFormCollection form)
      {
          string username = form["username"];
          string password = form["password"];

          HashAlgorithm sha = SHA256.Create();
          byte[] hash = sha.ComputeHash(
              Encoding.UTF8.GetBytes(username + password));
              
          User user = dbContext.Users.FirstOrDefault(x =>
              x.Username == username &&
              x.PassHash == hash
          );

          if (user == null) {
              return RedirectToAction("Index", "Login");
          }

          // create a new session and tag to user
          Session session = new Session() {
              User = user
          };
          dbContext.Sessions.Add(session);
          dbContext.SaveChanges();

          // ask browser to save and send back these cookies next time
          Response.Cookies.Append("SessionId", session.Id.ToString());
          Response.Cookies.Append("Username", user.Username);

          return RedirectToAction("Index", "Home");
      }
    }
}