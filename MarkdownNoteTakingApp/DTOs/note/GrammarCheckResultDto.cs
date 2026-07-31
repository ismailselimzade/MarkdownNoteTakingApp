namespace MarkdownNoteTakingApp.DTOs.note
{
    public class GrammarCheckResultDto
    {
        public string Message { get; set; }
        public List<string> Suggestions { get; set; }
    }
}
