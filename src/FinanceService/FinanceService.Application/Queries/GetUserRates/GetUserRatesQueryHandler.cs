using FinanceService.Application.Abstractions;
using FinanceService.Application.Dto;
using MediatR;

namespace FinanceService.Application.Queries.GetUserRates;

public class GetUserRatesQueryHandler : IRequestHandler<GetUserRatesQuery, IReadOnlyList<CurrencyRateDto>>
{
    private readonly IFavoriteCurrencyRepository _favorites;

    public GetUserRatesQueryHandler(IFavoriteCurrencyRepository favorites)
    {
        _favorites = favorites;
    }

    public async Task<IReadOnlyList<CurrencyRateDto>> Handle(GetUserRatesQuery request, CancellationToken cancellationToken)
    {
        var currencies = await _favorites.GetUserCurrenciesAsync(request.UserId, cancellationToken);
        return currencies.Select(CurrencyRateDto.From).ToList();
    }
}
