using System.ComponentModel.DataAnnotations;

namespace MarkdownNoteTakingApp.DTOs.user
{
    public class LoginDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
