using NSubstitute;
using UserService.Application.Abstractions;
using UserService.Application.Commands.Register;
using UserService.Domain.Entities;

namespace UserService.UnitTests.Commands;

public class RegisterUserCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _handler = new RegisterUserCommandHandler(_users, _passwordHasher);
    }

    [Fact]
    public async Task Handle_NewUser_SavesUserWithHashedPassword()
    {
        _passwordHasher.Hash("secret123").Returns("hashed");

        var result = await _handler.Handle(new RegisterUserCommand("alex", "secret123"), CancellationToken.None);

        Assert.NotNull(result);
        await _users.Received(1).AddAsync(
            Arg.Is<User>(u => u.Name == "alex" && u.PasswordHash == "hashed"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NameAlreadyTaken_ReturnsNull()
    {
        _users.ExistsAsync("alex", Arg.Any<CancellationToken>()).Returns(true);

        var result = await _handler.Handle(new RegisterUserCommand("alex", "secret123"), CancellationToken.None);

        Assert.Null(result);
        await _users.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }
}
