using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Task_Manager_API.DTOs;
using Task_Manager_API.Models;
using Task_Manager_API.Persistence;

namespace Task_Manager_API.Services
{
    public class AuthService(TaskDbContext context, IOptions<JwtSettings> jwtSettings, ILogger<AuthService> logger) : IAuthService
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;
        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            logger.LogInformation("Login attempt for user: {Username}", dto.Username);
            var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                logger.LogWarning("Invalid creds for user: {Username}", dto.Username);
                return null;
            }
            logger.LogInformation("User logged in successfully");
            return GenerateToken(user);
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            logger.LogInformation("Registering user: {Username}", dto.Username);
            var existingUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (existingUser is not null)
            {
                logger.LogWarning("{Username} already exists", dto.Username);
                throw new InvalidOperationException($"Username: {dto.Username} already taken");
            }

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            logger.LogInformation("{Username} registered successfully", dto.Username);
            return GenerateToken(user);
         }

        private AuthResponseDto GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

            var token = new JwtSecurityToken
                (
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: expiresAt,
                    signingCredentials: credentials
                );
            return new AuthResponseDto(
                Token: new JwtSecurityTokenHandler().WriteToken(token),
                Username: user.Username,
                ExpiresAt: expiresAt);
        }
    }
}
