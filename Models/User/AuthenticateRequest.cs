using System.ComponentModel.DataAnnotations;

namespace Boat_2.Models;

public class AuthenticateRequest
{
    [Required]
    public string? Username { get; set; }

    [Required]
    public string? Password { get; set; }
}
