using System.ComponentModel.DataAnnotations;

namespace MarkdownNoteTakingApp.DTOs.user
{
    public class DeleteUserDto
    {
        [Required]
        public string Password { get; set; }
    }
}
