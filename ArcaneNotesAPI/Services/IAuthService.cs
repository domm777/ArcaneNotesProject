using ArcaneNotesAPI.Entities;

namespace ArcaneNotesAPI.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(string email, string password);
    Task<(string?, string, string)> LoginAsync(string email, string password);
}