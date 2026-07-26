namespace MarkdownNoteTakingApp.Models
{
    public class Note
    {
        public Note()
        {
            Attachments = new HashSet<Attachment>();
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public User User { get; set; }
        public ICollection<Attachment> Attachments { get; set; }
    }
}
