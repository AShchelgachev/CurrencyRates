using MediatR;

namespace FinanceService.Application.Commands.AddFavorite;

public record AddFavoriteCurrencyCommand(int UserId, int CurrencyId) : IRequest<bool>;
