namespace MarkdownNoteTakingApp.Models
{
    public class User
    {
        public User()
        {
            Notes = new HashSet<Note>();
        }
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Note> Notes { get; set; }
    }
}
