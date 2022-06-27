using LiveChat.Data;
using LiveChat.Helpers;
using LiveChat.Models;
using LiveChat.Services;
using Microsoft.AspNetCore.Mvc;
using static LiveChat.Helpers.GlobalVariables;

namespace LiveChat.Controllers
{
    public class SharedController : Controller
    {
        private readonly UserService _userService;
        private readonly AppDbContext _context;


        public SharedController(UserService userService,AppDbContext context)
        {
            this._userService = userService;
            this._context = context;
        }

        public IActionResult _Layout()
        {
            return View(globalDynamic);
        }
    }
}
