using System.ComponentModel.DataAnnotations;

namespace LiveChat.Models
{
    public class ChatUsers
    {
        [Key]
        public int Id { get; set; }
        [Required,EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirthday { get; set; } = DateTime.Now; 
        [MaxLength(3)]
        public int Age { get; set; } = 0;
        [Required,MinLength(6)]
        public string Username { get; set; } = string.Empty;
        [Required,MinLength(9)]
        public string Password { get; set; } = string.Empty;
        public byte[] ProfileImage { get; set; } = null;
        public string FriendRequest { get; set; } = string.Empty;
        public string Friends { get; set; } = string.Empty;
    }
}