using FinanceService.Application.Abstractions;
using FinanceService.Domain.Entities;
using MediatR;

namespace FinanceService.Application.Commands.AddFavorite;

public class AddFavoriteCurrencyCommandHandler : IRequestHandler<AddFavoriteCurrencyCommand, bool>
{
    private readonly ICurrencyRepository _currencies;
    private readonly IFavoriteCurrencyRepository _favorites;

    public AddFavoriteCurrencyCommandHandler(ICurrencyRepository currencies, IFavoriteCurrencyRepository favorites)
    {
        _currencies = currencies;
        _favorites = favorites;
    }

    public async Task<bool> Handle(AddFavoriteCurrencyCommand request, CancellationToken cancellationToken)
    {
        if (!await _currencies.ExistsAsync(request.CurrencyId, cancellationToken))
        {
            return false;
        }

        if (!await _favorites.ExistsAsync(request.UserId, request.CurrencyId, cancellationToken))
        {
            await _favorites.AddAsync(new UserFavoriteCurrency(request.UserId, request.CurrencyId), cancellationToken);
        }

        return true;
    }
}
