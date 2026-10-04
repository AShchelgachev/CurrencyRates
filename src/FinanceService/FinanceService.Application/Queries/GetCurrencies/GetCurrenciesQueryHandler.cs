using FinanceService.Application.Abstractions;
using FinanceService.Application.Dto;
using MediatR;

namespace FinanceService.Application.Queries.GetCurrencies;

public class GetCurrenciesQueryHandler : IRequestHandler<GetCurrenciesQuery, IReadOnlyList<CurrencyRateDto>>
{
    private readonly ICurrencyRepository _currencies;

    public GetCurrenciesQueryHandler(ICurrencyRepository currencies)
    {
        _currencies = currencies;
    }

    public async Task<IReadOnlyList<CurrencyRateDto>> Handle(GetCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var currencies = await _currencies.GetAllAsync(cancellationToken);
        return currencies.Select(CurrencyRateDto.From).ToList();
    }
}
