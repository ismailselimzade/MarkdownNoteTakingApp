using MarkdownNoteTakingApp.Data;
using MarkdownNoteTakingApp.DTOs.attachment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarkdownNoteTakingApp.Models;

namespace MarkdownNoteTakingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttachmentsController : BaseController
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AttachmentsController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _db = appDbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> AddAttachment([FromForm] AddAttachmentDto addAttachmentDto)
        {
            var note = await _db.Notes.FirstOrDefaultAsync(n => n.UserId == UserId && n.Id == addAttachmentDto.NoteId);
            if (note == null) return NotFound();

            var attachment = new Attachment()
            {
                NoteId = note.Id,
                Title = addAttachmentDto.File.FileName,
                ContentType = addAttachmentDto.File.ContentType,
                Size = addAttachmentDto.File.Length,
                CreatedAt = DateTime.UtcNow,
                FilePath = Guid.NewGuid().ToString() + Path.GetExtension(addAttachmentDto.File.FileName)
            };

            var uploadsFolderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads");
            var filePath = Path.Combine(uploadsFolderPath, attachment.FilePath);

            using var stream = new FileStream(filePath, FileMode.Create);
            await addAttachmentDto.File.CopyToAsync(stream);

            await _db.AddAsync(attachment);
            await _db.SaveChangesAsync();

            var response = new GetAttachmentDto
            {
                Id = attachment.Id,
                NoteId = attachment.NoteId,
                ContentType = attachment.ContentType,
                Title = attachment.Title,
                Size = attachment.Size,
                CreatedAt = attachment.CreatedAt
            };

            return CreatedAtAction(nameof(DownloadAttachment), new { attachmentId = attachment.Id }, response);
        }

        [HttpGet("note/{noteId}")]
        public async Task<IActionResult> GetAttachments(int noteId)
        {
            var note = await _db.Notes
                .Include(n => n.Attachments)
                .FirstOrDefaultAsync(n => n.UserId == UserId && n.Id == noteId);
            if (note == null) return NotFound();

            var attachments = note.Attachments.Select(a => new GetAttachmentDto
            {
                Id = a.Id,
                NoteId = a.NoteId,
                Title = a.Title,
                ContentType = a.ContentType,
                Size = a.Size,
                CreatedAt = a.CreatedAt
            });

            return Ok(attachments);
        }

        [HttpGet("{attachmentId}")]
        public async Task<IActionResult> DownloadAttachment(int attachmentId)
        {
            var attachment = await _db.Attachments
                .Include(a => a.Note)
                .FirstOrDefaultAsync(a => a.Id == attachmentId && a.Note.UserId == UserId);
            if (attachment == null) return NotFound();

            var uploadsFolderPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads");
            var filePath = Path.Combine(uploadsFolderPath, attachment.FilePath);

            if (!System.IO.File.Exists(filePath)) return NotFound();

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            return File(stream, attachment.ContentType, attachment.Title);
        }

        [HttpDelete("{attachmentId}")]
        public async Task<IActionResult> DeleteAttachment(int attachmentId)
        {
            var attachment = await _db.Attachments
                .Include(a => a.Note)
                .FirstOrDefaultAsync(a => a.Id == attachmentId && a.Note.UserId == UserId);
            if (attachment == null) return NotFound();

            _db.Attachments.Remove(attachment);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
