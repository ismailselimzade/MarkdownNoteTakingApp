using System.ComponentModel.DataAnnotations;

namespace MarkdownNoteTakingApp.DTOs.note
{
    public class UpdateNoteDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
