using FinanceService.Application.Abstractions;
using MediatR;

namespace FinanceService.Application.Commands.RemoveFavorite;

public class RemoveFavoriteCurrencyCommandHandler : IRequestHandler<RemoveFavoriteCurrencyCommand>
{
    private readonly IFavoriteCurrencyRepository _favorites;

    public RemoveFavoriteCurrencyCommandHandler(IFavoriteCurrencyRepository favorites)
    {
        _favorites = favorites;
    }

    public async Task Handle(RemoveFavoriteCurrencyCommand request, CancellationToken cancellationToken)
    {
        await _favorites.RemoveAsync(request.UserId, request.CurrencyId, cancellationToken);
    }
}
