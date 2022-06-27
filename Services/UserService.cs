namespace LiveChat.Services
{
    using LiveChat.Data;
    using LiveChat.Helpers;
    using LiveChat.Models;
    using LiveChat.Models.User;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<ChatUsers> GetAll();
        ChatUsers GetById(int id);

    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly AppSettings _appSettings;
        public bool _LoggedIn = false;
        public string _Token = string.Empty;

        public UserService(IOptions<AppSettings> appSettings, AppDbContext context)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var users = _context.ChatUsers.FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password);
            if (users == null)
            {
                LoginOut();
                return null;
            }

            var token = GenerateJwtToken(users);
            _Token = token;
            LoginIn();
            return new AuthenticateResponse(users, token);

        }

        public bool Logged()
        {
            return _LoggedIn;
        }
        public string Token()
        {
            return _Token;
        }
        public void LoginIn()
        {
            _LoggedIn = true;
        }
        public void LoginOut()
        {
            _LoggedIn = false;
        }

        public IEnumerable<ChatUsers> GetAll()
        {
            return _context.ChatUsers.ToList();
        }

        public ChatUsers GetById(int id)
        {
            var user = _context.ChatUsers.FirstOrDefault(x => x.Id == id);
            if (user != null)
                return user;
            return (ChatUsers)Results.BadRequest("Not Found With this Id:" + id);
        }

        private string GenerateJwtToken(ChatUsers user)
        {
            //generate token 7 Days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var tokenDescriptior = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id",user.Id.ToString()),
                    new Claim("Email",user.Email),
                    new Claim("Username",user.Username),
                    new Claim("Age",user.Age.ToString()),
                    new Claim("FirstName",user.FirstName),
                    new Claim("LastName",user.LastName)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptior);
            return tokenHandler.WriteToken(token);
        }
    }
}
