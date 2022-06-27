using System.ComponentModel.DataAnnotations;

namespace LiveChat.Models.User
{
    public class Messages
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int SenderId { get; set; }
        [Required]
        public int RecipientId { get; set; }
        public DateTime DateTime { get;} = DateTime.Now;
        [Required]
        public string Message { get; set; } = string.Empty;
        public int Liked { get; set; }
    }
}
