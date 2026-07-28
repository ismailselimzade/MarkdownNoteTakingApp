using System.ComponentModel.DataAnnotations;

namespace MarkdownNoteTakingApp.DTOs.user
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
