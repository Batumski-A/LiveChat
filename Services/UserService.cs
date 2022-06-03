using Boat_2.Data;
using Boat_2.Models;
using LiveChat.Helpers;
using LiveChat.Models.User;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace LiveChat.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;
        private readonly AppSettings _appSettings;

        public UserService(AppDbContext context,AppSettings appSettings)
        {
              _context = context;
            _appSettings = appSettings;
        }

      /*  public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _context.users.FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password);

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt token
            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }*/

        public IEnumerable<Users> GetAll()
        {
            return _context.users.ToList();
        }

        public Users GetById(int id)
        {
            var user = _context.users.FirstOrDefault(x => x.Id == id);
            if (user != null)
                return user;
            return (Users)Results.BadRequest("");
        }

       /* private string generateJwtToken(Users user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()), new Claim("Username", user.Username.ToString()), new Claim("FirstName", user.FirstName.ToString()), new Claim("LastName", user.LastName.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }*/

    }
}
