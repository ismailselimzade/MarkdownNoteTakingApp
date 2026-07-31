using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace MarkdownNoteTakingApp.DTOs.attachment
{
    public class AddAttachmentDto
    {
        [Required]
        public int NoteId { get; set; }
        [Required]
        public IFormFile File { get; set; }
    }
}
