using LiveChat.Data;
using LiveChat.Services;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using static LiveChat.Helpers.GlobalVariables;

namespace LiveChat.Controllers
{
    public class PublicController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;
        public PublicController(AppDbContext appDbContext,UserService userService)
        {
            _context = appDbContext;
            _userService = userService;
        }
        [HttpGet]
        public IActionResult FindFriend()
        {
            
            return View(globalDynamic);
        }
    }
}
