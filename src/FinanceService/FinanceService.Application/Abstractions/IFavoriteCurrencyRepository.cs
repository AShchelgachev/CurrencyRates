using FinanceService.Domain.Entities;

namespace FinanceService.Application.Abstractions;

public interface IFavoriteCurrencyRepository
{
    Task<IReadOnlyList<Currency>> GetUserCurrenciesAsync(int userId, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int userId, int currencyId, CancellationToken cancellationToken);

    Task AddAsync(UserFavoriteCurrency favorite, CancellationToken cancellationToken);

    Task RemoveAsync(int userId, int currencyId, CancellationToken cancellationToken);
}
