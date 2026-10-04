using FinanceService.Application.Abstractions;
using FinanceService.Application.Commands.AddFavorite;
using FinanceService.Domain.Entities;
using NSubstitute;

namespace FinanceService.UnitTests.Commands;

public class AddFavoriteCurrencyCommandHandlerTests
{
    private readonly ICurrencyRepository _currencies = Substitute.For<ICurrencyRepository>();
    private readonly IFavoriteCurrencyRepository _favorites = Substitute.For<IFavoriteCurrencyRepository>();
    private readonly AddFavoriteCurrencyCommandHandler _handler;

    public AddFavoriteCurrencyCommandHandlerTests()
    {
        _handler = new AddFavoriteCurrencyCommandHandler(_currencies, _favorites);
    }

    [Fact]
    public async Task Handle_NewFavorite_AddsIt()
    {
        _currencies.ExistsAsync(10, Arg.Any<CancellationToken>()).Returns(true);
        _favorites.ExistsAsync(1, 10, Arg.Any<CancellationToken>()).Returns(false);

        var result = await _handler.Handle(new AddFavoriteCurrencyCommand(1, 10), CancellationToken.None);

        Assert.True(result);
        await _favorites.Received(1).AddAsync(
            Arg.Is<UserFavoriteCurrency>(f => f.UserId == 1 && f.CurrencyId == 10),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AlreadyFavorite_DoesNotAddAgain()
    {
        _currencies.ExistsAsync(10, Arg.Any<CancellationToken>()).Returns(true);
        _favorites.ExistsAsync(1, 10, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _handler.Handle(new AddFavoriteCurrencyCommand(1, 10), CancellationToken.None);

        Assert.True(result);
        await _favorites.DidNotReceive().AddAsync(Arg.Any<UserFavoriteCurrency>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownCurrency_ReturnsFalse()
    {
        _currencies.ExistsAsync(999, Arg.Any<CancellationToken>()).Returns(false);

        var result = await _handler.Handle(new AddFavoriteCurrencyCommand(1, 999), CancellationToken.None);

        Assert.False(result);
        await _favorites.DidNotReceive().AddAsync(Arg.Any<UserFavoriteCurrency>(), Arg.Any<CancellationToken>());
    }
}
