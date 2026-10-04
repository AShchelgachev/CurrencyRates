using NSubstitute;
using UserService.Application.Abstractions;
using UserService.Application.Commands.Login;
using UserService.Application.Dto;
using UserService.Domain.Entities;

namespace UserService.UnitTests.Commands;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenGenerator _tokenGenerator = Substitute.For<IJwtTokenGenerator>();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_users, _passwordHasher, _tokenGenerator);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        var user = new User("alex", "hashed");
        var token = new AccessToken("jwt", DateTime.UtcNow.AddHours(1));
        _users.GetByNameAsync("alex", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("hashed", "secret123").Returns(true);
        _tokenGenerator.Generate(user).Returns(token);

        var result = await _handler.Handle(new LoginCommand("alex", "secret123"), CancellationToken.None);

        Assert.Equal(token, result);
    }

    [Fact]
    public async Task Handle_UnknownUser_ReturnsNull()
    {
        _users.GetByNameAsync("alex", Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _handler.Handle(new LoginCommand("alex", "secret123"), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WrongPassword_ReturnsNull()
    {
        _users.GetByNameAsync("alex", Arg.Any<CancellationToken>()).Returns(new User("alex", "hashed"));
        _passwordHasher.Verify("hashed", "wrong").Returns(false);

        var result = await _handler.Handle(new LoginCommand("alex", "wrong"), CancellationToken.None);

        Assert.Null(result);
        _tokenGenerator.DidNotReceive().Generate(Arg.Any<User>());
    }
}
