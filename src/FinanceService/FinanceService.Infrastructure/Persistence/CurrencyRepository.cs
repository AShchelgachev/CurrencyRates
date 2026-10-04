using FinanceService.Application.Abstractions;
using FinanceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Persistence;

public class CurrencyRepository : ICurrencyRepository
{
    private readonly FinanceDbContext _db;

    public CurrencyRepository(FinanceDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Currency>> GetAllAsync(CancellationToken cancellationToken) =>
        await _db.Currencies
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) =>
        _db.Currencies.AnyAsync(c => c.Id == id, cancellationToken);
}
