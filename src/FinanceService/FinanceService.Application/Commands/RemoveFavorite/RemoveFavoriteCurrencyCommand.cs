using MediatR;

namespace FinanceService.Application.Commands.RemoveFavorite;

public record RemoveFavoriteCurrencyCommand(int UserId, int CurrencyId) : IRequest;
