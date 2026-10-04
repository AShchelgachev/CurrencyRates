using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstractions;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence;

public class RevokedTokenRepository : IRevokedTokenRepository
{
    private readonly UserDbContext _db;

    public RevokedTokenRepository(UserDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(RevokedToken token, CancellationToken cancellationToken)
    {
        _db.RevokedTokens.Add(token);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken) =>
        _db.RevokedTokens.AnyAsync(t => t.Jti == jti, cancellationToken);
}
