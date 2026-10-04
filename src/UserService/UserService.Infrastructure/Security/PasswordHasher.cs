using Microsoft.AspNetCore.Identity;
using UserService.Application.Abstractions;

namespace UserService.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password) =>
        _hasher.HashPassword(null!, password);

    public bool Verify(string passwordHash, string password) =>
        _hasher.VerifyHashedPassword(null!, passwordHash, password) != PasswordVerificationResult.Failed;
}
