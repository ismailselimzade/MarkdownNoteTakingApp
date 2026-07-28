using System.ComponentModel.DataAnnotations;

namespace MarkdownNoteTakingApp.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
