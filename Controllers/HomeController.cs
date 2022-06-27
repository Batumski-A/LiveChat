using LiveChat.Helpers;
using LiveChat.Models;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using static LiveChat.Helpers.GlobalVariables;

namespace LiveChat.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(globalDynamic);
        }
    }
}
