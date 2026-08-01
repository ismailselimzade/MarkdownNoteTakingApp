using Markdig;
using MarkdownNoteTakingApp.Data;
using MarkdownNoteTakingApp.DTOs.note;
using MarkdownNoteTakingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MarkdownNoteTakingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotesController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly IHttpClientFactory _http;

        public NotesController(AppDbContext appDbContext, IHttpClientFactory httpClient)
        {
            _db = appDbContext;
            _http = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotes()
        {
            var user = await _db.Users
                .Include(u => u.Notes)
                .FirstOrDefaultAsync(u => u.Id == UserId);

            if (user == null) return NotFound();

            var notes = user.Notes.Select(note => new GetNoteResponseDto
            {
                Id = note.Id,
                UserId = note.UserId,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt
            }).ToList();

            return Ok(notes);
        }

        
        [HttpGet("{noteId}")]
        public async Task<IActionResult> GetNote(int noteId)
        {
            var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == noteId && n.UserId == UserId);

            if (note == null) return NotFound();

            var response = new GetNoteResponseDto
            {
                Id = note.Id,
                UserId = note.UserId,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt
            };

            return Ok(response);
        }

        
        [HttpPost]
        public async Task<IActionResult> AddNote(AddNoteDto addNoteDto)
        {
            var note = new Note
            {
                UserId = UserId,
                Title = addNoteDto.Title,
                Content = addNoteDto.Content,
                CreatedAt = DateTime.UtcNow
            };

            await _db.Notes.AddAsync(note);
            await _db.SaveChangesAsync();

            var response = new GetNoteResponseDto
            {
                Id = note.Id,
                UserId = note.UserId,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt
            };

            return CreatedAtAction(nameof(GetNote), new { noteId = note.Id }, response);
        }

        
        [HttpPut("{noteId}")]
        public async Task<IActionResult> UpdateNote(UpdateNoteDto updateNoteDto, int noteId)
        {
            var note = await _db.Notes.FirstOrDefaultAsync(u => u.Id == noteId && u.UserId == UserId);

            if (note == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(updateNoteDto.Title))
                note.Title = updateNoteDto.Title;

            if (!string.IsNullOrWhiteSpace(updateNoteDto.Content))
                note.Content = updateNoteDto.Content;

            await _db.SaveChangesAsync();

            var response = new GetNoteResponseDto
            {
                Id = note.Id,
                UserId = note.UserId,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt
            };

            return Ok(response);
        }

        [HttpDelete("{noteId}")]
        public async Task<IActionResult> DeleteNote(int noteId) 
        {
            var note = await _db.Notes.FirstOrDefaultAsync(n => n.UserId == UserId && n.Id == noteId);

            if (note == null) return NotFound();

            _db.Notes.Remove(note);
            await _db.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("render/{noteId}")]
        public async Task<IActionResult> GetNoteRender(int noteId)
        {
            var note = await _db.Notes.FirstOrDefaultAsync(n => n.UserId == UserId && n.Id == noteId);
            if (note == null) return NotFound();

            var content = note.Content;
            var render = Markdown.ToHtml(content);

            return Content(render, "text/html");
        }
        

        [HttpGet("grammar-check/{noteId}")]
        public async Task<IActionResult> GetNoteGrammarCheck(int noteId)
        {
            var note = await _db.Notes.FirstOrDefaultAsync(n => n.UserId == UserId && n.Id == noteId);
            if (note == null) return NotFound();

            var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("text", note.Content),
                new KeyValuePair<string, string>("language", "auto")
            });

            var client = _http.CreateClient();

            var responseApi = await client.PostAsync("https://api.languagetool.org/v2/check", content);

            var json = await responseApi.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LanguageToolResponseDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null) return NotFound();

            var response = result.Matches.Select(m => new GrammarCheckResultDto
            {
                Message = m.Message,
                Suggestions = m.Replacements.Select(r => r.Value).ToList()
            }).ToList();

            return Ok(response);
        }
    }
}
