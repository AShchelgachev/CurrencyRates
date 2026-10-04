using FinanceService.Application.Dto;
using MediatR;

namespace FinanceService.Application.Queries.GetUserRates;

public record GetUserRatesQuery(int UserId) : IRequest<IReadOnlyList<CurrencyRateDto>>;
