using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ArcaneNotesAPI.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace ArcaneNotesAPI.Services;

public class AuthService : IAuthService
{
    private readonly IMongoCollection<User> _users;
    private readonly JWTSettings _jwtSettings;

    public AuthService(IMongoClient client, IOptions<JWTSettings> jwtSettings)
    {
        var database = client.GetDatabase("ArcaneNotesDB");
        _users = database.GetCollection<User>("Users");
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<User?> RegisterAsync(string email, string password)
    {
        string normalizedEmail = email.Trim().ToLowerInvariant();
        var existing = await _users.Find(u => u.Email == normalizedEmail).FirstOrDefaultAsync();
        if (existing != null) return null;

        var user = new User
        {
            Email = normalizedEmail,
            Username = normalizedEmail.Split('@')[0],
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        await _users.InsertOneAsync(user);
        return user;
    }

    public async Task<(string?, string?, string?)> LoginAsync(string email, string password)
    {
        string normalizedEmail = email.Trim().ToLowerInvariant();
        
        var user = await _users.Find(u => u.Email == normalizedEmail).FirstOrDefaultAsync();

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return (null, null, null);
        string token = GenerateJwtToken(user);
        
        return (token, user.Username, user.Id);
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}