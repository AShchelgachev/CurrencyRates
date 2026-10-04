using FinanceService.Domain.Entities;

namespace FinanceService.Application.Dto;

public record CurrencyRateDto(int Id, string Name, decimal Rate)
{
    public static CurrencyRateDto From(Currency currency) => new(currency.Id, currency.Name, currency.Rate);
}
