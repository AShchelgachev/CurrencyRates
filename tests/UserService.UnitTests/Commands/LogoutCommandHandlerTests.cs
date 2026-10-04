using NSubstitute;
using UserService.Application.Abstractions;
using UserService.Application.Commands.Logout;
using UserService.Domain.Entities;

namespace UserService.UnitTests.Commands;

public class LogoutCommandHandlerTests
{
    [Fact]
    public async Task Handle_SavesTokenIdAsRevoked()
    {
        var revokedTokens = Substitute.For<IRevokedTokenRepository>();
        var handler = new LogoutCommandHandler(revokedTokens);
        var expiresAt = new DateTime(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);

        await handler.Handle(new LogoutCommand("token-id", expiresAt), CancellationToken.None);

        await revokedTokens.Received(1).AddAsync(
            Arg.Is<RevokedToken>(t => t.Jti == "token-id" && t.ExpiresAt == expiresAt),
            Arg.Any<CancellationToken>());
    }
}
