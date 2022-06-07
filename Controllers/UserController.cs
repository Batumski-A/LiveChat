using LiveChat.Data;
using LiveChat.Models;
using LiveChat.Services;
using Microsoft.AspNetCore.Mvc;

namespace LiveChat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userservice;
        private readonly AppDbContext _context;

        public UserController(UserService Userservice,AppDbContext context)
        {
            _userservice =  Userservice;
            _context = context;
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate(string Username,string Password)
        {
            var response = _userservice.Authenticate(Username,Password);

            if (response == null)
            {
                return BadRequest(new { message = "Username or Password is incorrect" });
            }
            return Ok(response);
        }

        [HttpPost]
        public Task<ChatUsers> Registration(string firstName,string lastName,string email, int age,string username,string password )
        {

            ChatUsers newUser = new()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Age = age,
                DateOfBirthday = DateTime.Today,
                Username = username,
                Password = password,
            };
            System.Diagnostics.Debug.WriteLine(newUser + "-----------------------");


            _context.ChatUsers.Add(newUser);
            _context.SaveChanges();
            return Task.FromResult(newUser);        
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var ChatUsers = _userservice.GetAll();
            return Ok(ChatUsers);
        }

    }
}
