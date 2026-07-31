using System.ComponentModel.DataAnnotations;

namespace MarkdownNoteTakingApp.DTOs.note
{
    public class AddNoteDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
