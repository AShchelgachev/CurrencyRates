using FinanceService.Application.Abstractions;
using FinanceService.Application.Dto;
using FinanceService.Application.Queries.GetUserRates;
using FinanceService.Domain.Entities;
using NSubstitute;

namespace FinanceService.UnitTests.Queries;

public class GetUserRatesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsRatesOfUserFavoriteCurrencies()
    {
        var favorites = Substitute.For<IFavoriteCurrencyRepository>();
        favorites.GetUserCurrenciesAsync(1, Arg.Any<CancellationToken>()).Returns(new[]
        {
            new Currency(10, "EUR", 94.3201m),
            new Currency(20, "USD", 83.4839m)
        });
        var handler = new GetUserRatesQueryHandler(favorites);

        var result = await handler.Handle(new GetUserRatesQuery(1), CancellationToken.None);

        Assert.Equal(
            new[] { new CurrencyRateDto(10, "EUR", 94.3201m), new CurrencyRateDto(20, "USD", 83.4839m) },
            result);
    }
}
