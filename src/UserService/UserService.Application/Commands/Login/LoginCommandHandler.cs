using MediatR;
using UserService.Application.Abstractions;
using UserService.Application.Dto;

namespace UserService.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AccessToken?>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public LoginCommandHandler(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AccessToken?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByNameAsync(request.Name, cancellationToken);

        if (user is null || !_passwordHasher.Verify(user.PasswordHash, request.Password))
        {
            return null;
        }

        return _tokenGenerator.Generate(user);
    }
}
