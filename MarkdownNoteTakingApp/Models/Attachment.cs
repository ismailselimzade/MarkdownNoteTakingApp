namespace MarkdownNoteTakingApp.Models
{
    public class Attachment
    {
        public int Id { get; set; }
        public int NoteId { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public Note Note { get; set; }
    }
}
