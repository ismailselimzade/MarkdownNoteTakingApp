namespace MarkdownNoteTakingApp.DTOs.attachment
{
    public class GetAttachmentDto
    {
        public int Id { get; set; }
        public int NoteId { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
