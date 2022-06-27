using LiveChat.Data;
using LiveChat.Models;
using LiveChat.Models.User;
using LiveChat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using static LiveChat.Helpers.GlobalVariables;

namespace LiveChat.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userservice;
        private readonly AppDbContext _context;
        public static ChatUsers? LoggedUser;
        private bool _CheckeRegistrationIs = true;

        public UserController(UserService Userservice, AppDbContext context)
        {
            _userservice = Userservice;
            _context = context;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Registration()
        {
            return View("~/Views/User/Registration.cshtml");
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Registration(string FirstName, string LastName, string Email, string UserName, string Password, int Age, IFormFile profileImage, [FromServices] IWebHostEnvironment hostingEnvironment)
        {
            string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{profileImage.FileName}";
            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                profileImage.CopyTo(fileStream);
                fileStream.Flush();
            }
            byte[] imageArray = System.IO.File.ReadAllBytes(fileName);
            ChatUsers chatUsers = new ChatUsers()
            {
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Username = UserName,
                Password = Password,
                Age = Age,
                ProfileImage = imageArray
            };
            string[] errors = CheckRegistration(chatUsers);
            dynamic dx = new ExpandoObject();
            dx.chatUsers = chatUsers;
            dx.errors = errors;
            dx.logged = _userservice.Logged();
            globalDynamic = dx;
            System.IO.File.Delete(fileName);

            if (_CheckeRegistrationIs)
            {
                dx.succsess = "You have successfully registered";
                System.Diagnostics.Debug.WriteLine("=============");
                _context.ChatUsers.Add(chatUsers);
                _context.SaveChanges();
                return View("~/Views/Home/index.cshtml", globalDynamic);
            }
            dx.succsess = "Not successfully";
            return View("~/Views/User/Registration.cshtml", globalDynamic);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userservice.Authenticate(model);
            TempData["msg"] = "Username or password incorrect";
            if (response == null)
            {
                TempData["msg"] = "Username or password incorrect";
                return View("~/Views/Home/Index.cshtml");
            }
            else
            {

                dynamic dy = new ExpandoObject();
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(response.Token);
                dynamic logedUser = JObject.Parse(jsonToken.ToString().Split("}.")[1]);
                //thisUser
                string thisUserId = logedUser.Id.ToString();
                User(int.Parse(thisUserId));
                //\thisUser
                dy.thisUser = LoggedUser;
                dy.succsess = "You have successfully logged in";
                dy.logged = _userservice.Logged();
                dy.AllUsers = GetAll();
                dy.FriendsRequest = GetAllRequest();
                dy.Friends = Friends();
                dy.FriendId = 2;
                globalDynamic = dy;
                return View("~/Views/Home/Index.cshtml", globalDynamic);
            }
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(globalDynamic);
        }

        [HttpGet]
        public IActionResult Message(int friendId)
        {
            List<Messages> messages = new List<Messages>();
            foreach (var message in _context.Messages.ToList())
            {
                if (messages.FirstOrDefault(m=>m.Id == message.Id)==null && ((message.SenderId == LoggedUser.Id && message.RecipientId == friendId)||(message.SenderId==friendId && message.RecipientId==LoggedUser.Id)))
                {
                    messages.Add(message);
                }
            }
            globalDynamic.FriendId = friendId;
            globalDynamic.Messages = messages;
            return View("~/Views/User/Index.cshtml", globalDynamic);

        }

        [HttpGet]
        public IActionResult UserFriends()
        {
            return View("~/Views/User/UserFriends.cshtml", globalDynamic);
        }

        [HttpGet]
        public IActionResult AddFriend(int friendId, string thisUserId)
        {
            ChatUsers users = _context.ChatUsers.FirstOrDefault(user => user.Id == friendId);
            if (users !=null)
            {
                if (!users.FriendRequest.Contains(String.Format("{0}|", thisUserId)))
                {
                    users.FriendRequest += thisUserId.ToString() + "|";
                    _context.ChatUsers.Update(users);
                    _context.SaveChanges();
                }
            }
            globalDynamic.FriendsRequest = GetAllRequest();
            return View("~/Views/Public/FindFriend.cshtml", globalDynamic);
        }

        public List<ChatUsers> GetAll()
        {
            List<ChatUsers> ChatUsers = _userservice.GetAll().ToList();
            ChatUsers.Remove(LoggedUser);
            return ChatUsers;
        }

        public List<ChatUsers> GetAllRequest()
        {
            string allRequest = LoggedUser.FriendRequest;
            List<ChatUsers> friendRequests = new List<ChatUsers>();
            if (allRequest.Length > 1)
            {
                foreach (string result in allRequest.Split("|"))
                {
                    if(result.Length > 0)
                    {
                        ChatUsers User = _context.ChatUsers.FirstOrDefault(user => user.Id == int.Parse(result));
                        if(User != null & User.Id != LoggedUser.Id)
                        {
                            friendRequests.Add(User);
                        }
                    }
                }
            }
            
            return friendRequests;
        }

        public List<ChatUsers> Friends()
        {
            string allFriends = LoggedUser.Friends;
            List<ChatUsers> friends = new List<ChatUsers>();
            if (allFriends.Length > 1)
            {
                foreach (string result in allFriends.Split("|"))
                {
                    if (result.Length > 0)
                    {
                        ChatUsers User = _context.ChatUsers.FirstOrDefault(user => user.Id == int.Parse(result));
                        if (User != null & User.Id != LoggedUser.Id)
                        {
                            friends.Add(User);
                        }
                    }
                }
            }
            return friends;
        }

        [HttpGet]
        public IActionResult SendMessage(string message,int friend)
        {
            Messages mes = new Messages();
            mes.Message = message;
            mes.SenderId = LoggedUser.Id;
            mes.RecipientId = friend;
            _context.Messages.Add(mes);
            _context.SaveChanges();
            Message(friend);
            return View("~/Views/User/Index.cshtml", globalDynamic);
        }

        public AuthenticateResponse GetUser(AuthenticateRequest model)
        {
            var response = _userservice.Authenticate(model);
            return response;
        }

        public string[] CheckRegistration(ChatUsers user)
        {

            string[] vs = new string[6];
            _CheckeRegistrationIs = true;
            if (user.Email == null || _context.ChatUsers.FirstOrDefault(u => u.Email == user.Email) != null)
            {
                vs[0] = "Already registered with this email";
                _CheckeRegistrationIs = false;
            }
            if (user.FirstName == null)
            {
                vs[1] = "Please use only Latin letters";
                _CheckeRegistrationIs = false;
            }
            if (user.LastName == null)
            {
                vs[2] = "Please use only Latin letters";
                _CheckeRegistrationIs = false;
            }
            if (user.Age > 90 || user.Age < 12)
            {
                vs[3] = "Age should not exceed 90 and should not be less than 12";
                _CheckeRegistrationIs = false;
            }
            if (user.Username == null || user.Username.Contains(" ") || _context.ChatUsers.FirstOrDefault(u => u.Username == user.Username) != null)
            {
                vs[4] = "Used or we can not use this username";
                _CheckeRegistrationIs = false;
            }
            if (user.Password == null || user.Password.Length < 8)
            {
                vs[5] = "Password length must be more than 8 letters";
                _CheckeRegistrationIs = false;
            }
            return vs;
        }

        [HttpGet]
        public IActionResult FriendshipOffer(bool Agre,int offeredUserId)
        {
            ChatUsers offeredUser = new ChatUsers();
            offeredUser = _context.ChatUsers.FirstOrDefault(user=>user.Id == offeredUserId);
            if (Agre & offeredUser != null)
            {
                offeredUser.FriendRequest = offeredUser.FriendRequest.Replace(string.Format("{0}|", LoggedUser.Id), "");
                offeredUser.Friends = string.Format("{0}|", LoggedUser.Id);
                LoggedUser.FriendRequest = LoggedUser.FriendRequest.Replace(string.Format("{0}|", offeredUser.Id), "");
                LoggedUser.Friends += string.Format("{0}|", offeredUser.Id);
                _context.ChatUsers.Update(offeredUser);
                _context.ChatUsers.Update(LoggedUser);
                _context.SaveChanges();
            }
            else if(offeredUser != null)
            {
/*                offeredUser.FriendRequest = offeredUser.FriendRequest.Replace(string.Format("{0}|", LoggedUser.Id), "");
*/                LoggedUser.FriendRequest = LoggedUser.FriendRequest.Replace(string.Format("{0}|", offeredUser.Id), "");
                _context.ChatUsers.Update(offeredUser);
                _context.ChatUsers.Update(LoggedUser);
                _context.SaveChanges();
            }

            globalDynamic.FriendsRequest = GetAllRequest();
            return View("~/Views/Public/FindFriend.cshtml", globalDynamic);
        }

        private new void User(int id)
        {
            LoggedUser = _context.ChatUsers.FirstOrDefault(u => u.Id == id);
        }
    }
}
