using FinanceService.Application.Abstractions;
using FinanceService.Application.Commands.RemoveFavorite;
using NSubstitute;

namespace FinanceService.UnitTests.Commands;

public class RemoveFavoriteCurrencyCommandHandlerTests
{
    [Fact]
    public async Task Handle_RemovesFavoriteOfThisUser()
    {
        var favorites = Substitute.For<IFavoriteCurrencyRepository>();
        var handler = new RemoveFavoriteCurrencyCommandHandler(favorites);

        await handler.Handle(new RemoveFavoriteCurrencyCommand(1, 10), CancellationToken.None);

        await favorites.Received(1).RemoveAsync(1, 10, Arg.Any<CancellationToken>());
    }
}
