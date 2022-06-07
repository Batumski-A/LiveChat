
namespace LiveChat.Services
{
    using LiveChat.Data;
    using LiveChat.Models;
    using LiveChat.Helpers;
    using LiveChat.Models.User;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public interface IUserService
    {
        AuthenticateResponse Authenticate(string username,string password);
        IEnumerable<ChatUsers> GetAll();
        ChatUsers GetById(int id);
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings,AppDbContext context)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }
     
        public AuthenticateResponse Authenticate(string userName,string password)
        {
            var users = _context.ChatUsers.FirstOrDefault(x => x.Username == userName && x.Password == password);
            if (users == null) return null;

            var token = GenerateJwtToken(users);

            return new AuthenticateResponse(users, token);

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
            System.Diagnostics.Debug.WriteLine("----------------------------");

            var tokenDescriptior = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                {
                    new Claim(ClaimTypes.NameIdentifier,"id",user.Id.ToString()),
                    new Claim(ClaimTypes.NameIdentifier,user.Email),
                    new Claim(ClaimTypes.NameIdentifier,user.Username),
                    new Claim(ClaimTypes.NameIdentifier,user.Age.ToString()),
                    new Claim(ClaimTypes.NameIdentifier,user.FirstName),
                    new Claim(ClaimTypes.NameIdentifier,user.LastName)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptior);
            return tokenHandler.WriteToken(token);
        }
    }
}
