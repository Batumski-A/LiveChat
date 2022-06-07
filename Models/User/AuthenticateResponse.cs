
namespace LiveChat.Models.User
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime DateOfBirthday { get; set; } = DateTime.Now;
        public int Age { get; set; } = 0;

        public AuthenticateResponse(ChatUsers user, string token)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            Email = user.Email;
            DateOfBirthday = user.DateOfBirthday;
            Age = user.Age;
            Token = token;
        }
    }
}
