using FinanceService.Application.Abstractions;
using FinanceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Persistence;

public class FavoriteCurrencyRepository : IFavoriteCurrencyRepository
{
    private readonly FinanceDbContext _db;

    public FavoriteCurrencyRepository(FinanceDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Currency>> GetUserCurrenciesAsync(int userId, CancellationToken cancellationToken) =>
        await _db.UserFavoriteCurrencies
            .Where(f => f.UserId == userId)
            .Join(_db.Currencies, f => f.CurrencyId, c => c.Id, (f, c) => c)
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsAsync(int userId, int currencyId, CancellationToken cancellationToken) =>
        _db.UserFavoriteCurrencies.AnyAsync(f => f.UserId == userId && f.CurrencyId == currencyId, cancellationToken);

    public async Task AddAsync(UserFavoriteCurrency favorite, CancellationToken cancellationToken)
    {
        _db.UserFavoriteCurrencies.Add(favorite);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public Task RemoveAsync(int userId, int currencyId, CancellationToken cancellationToken) =>
        _db.UserFavoriteCurrencies
            .Where(f => f.UserId == userId && f.CurrencyId == currencyId)
            .ExecuteDeleteAsync(cancellationToken);
}
