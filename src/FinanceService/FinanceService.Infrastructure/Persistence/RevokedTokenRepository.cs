using FinanceService.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Persistence;

public class RevokedTokenRepository : IRevokedTokenRepository
{
    private readonly FinanceDbContext _db;

    public RevokedTokenRepository(FinanceDbContext db)
    {
        _db = db;
    }

    public Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken) =>
        _db.RevokedTokens.AnyAsync(t => t.Jti == jti, cancellationToken);
}
