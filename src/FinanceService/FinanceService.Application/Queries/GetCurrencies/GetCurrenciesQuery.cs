using FinanceService.Application.Dto;
using MediatR;

namespace FinanceService.Application.Queries.GetCurrencies;

public record GetCurrenciesQuery : IRequest<IReadOnlyList<CurrencyRateDto>>;
