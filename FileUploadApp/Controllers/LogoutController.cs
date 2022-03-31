using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadApp.Controllers
{
    public class LogoutController : Controller
    {
        public IActionResult Index()
        {
            // ask client to remove these cookies so that
            // they won't be sent over next time
            Response.Cookies.Delete("SessionId");
            Response.Cookies.Delete("Username");

            return RedirectToAction("Index", "Login");
        }
    }
}
