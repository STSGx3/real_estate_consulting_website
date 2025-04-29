using BTL_Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BTL_Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            return View();
        }
    }

}
