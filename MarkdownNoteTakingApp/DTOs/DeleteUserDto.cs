using System.ComponentModel.DataAnnotations;

namespace MarkdownNoteTakingApp.DTOs
{
    public class DeleteUserDto
    {
        [Required]
        public string Password { get; set; }
    }
}
