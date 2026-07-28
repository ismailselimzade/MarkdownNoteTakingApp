using MarkdownNoteTakingApp.Data;
using MarkdownNoteTakingApp.DTOs.user;
using MarkdownNoteTakingApp.Models;
using MarkdownNoteTakingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MarkdownNoteTakingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuthController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IPasswordService _passwordService;

        public AuthController(AppDbContext appDbContext, IConfiguration configuration, IPasswordService passwordService)
        {
            _db = appDbContext;
            _configuration = configuration;
            _passwordService = passwordService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (await _db.Users.AnyAsync(u => u.Username == registerDto.Username))
                return Conflict();

            var user = new User()
            {
                Username = registerDto.Username,
                PasswordHash = _passwordService.HashPassword(registerDto.Password),
                CreatedAt = DateTime.UtcNow
            };
            
            await _db.AddAsync(user);
            await _db.SaveChangesAsync();

            var response = new RegisterResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                CreatedAt = user.CreatedAt
            };

            return StatusCode(201, response);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _db.Users
                .Where(u => u.Username == loginDto.Username)
                .FirstOrDefaultAsync();

            if (user == null || !_passwordService.VerifyPassword(loginDto.Password, user.PasswordHash))
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiryMinutes"])),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials
                (
                    new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                    SecurityAlgorithms.HmacSha256
                )
            };

            var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new {token = jwtToken});
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _db.Users
                .Where(u => u.Id == userId)
                .Select(u => new GetProfileResponseDto
                {
                    Id = u.Id, Username = u.Username, CreatedAt = u.CreatedAt
                })
                .FirstOrDefaultAsync();

            return user == null ? NotFound() : Ok(user);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser(DeleteUserDto deleteUserDto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _db.Users.FindAsync(userId);

            if (user == null || !_passwordService.VerifyPassword(deleteUserDto.Password, user.PasswordHash))
                return Unauthorized();

            _db.Remove(user);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(UpdateUserDto updateUserDto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _db.Users.FindAsync(userId);

            if (user == null || !_passwordService.VerifyPassword(updateUserDto. OldPassword, user.PasswordHash))
                return Unauthorized();

            if (await _db.Users.AnyAsync(u => u.Username == updateUserDto.Username && u.Id != user.Id))
                return Conflict();

            user.Username = updateUserDto.Username;
            if (!string.IsNullOrWhiteSpace(updateUserDto.NewPassword))
                user.PasswordHash = _passwordService.HashPassword(updateUserDto.NewPassword);

            _db.Update(user);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
