using FinanceService.Application.Abstractions;
using FinanceService.Application.Dto;
using FinanceService.Application.Queries.GetCurrencies;
using FinanceService.Domain.Entities;
using NSubstitute;

namespace FinanceService.UnitTests.Queries;

public class GetCurrenciesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsAllCurrencies()
    {
        var currencies = Substitute.For<ICurrencyRepository>();
        currencies.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new[]
        {
            new Currency(1, "DZD", 0.62466m),
            new Currency(2, "USD", 83.4839m)
        });
        var handler = new GetCurrenciesQueryHandler(currencies);

        var result = await handler.Handle(new GetCurrenciesQuery(), CancellationToken.None);

        Assert.Equal(
            new[] { new CurrencyRateDto(1, "DZD", 0.62466m), new CurrencyRateDto(2, "USD", 83.4839m) },
            result);
    }
}
