using MediatR;
using UserService.Application.Abstractions;
using UserService.Domain.Entities;

namespace UserService.Application.Commands.Register;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, int?>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(IUserRepository users, IPasswordHasher passwordHasher)
    {
        _users = users;
        _passwordHasher = passwordHasher;
    }

    public async Task<int?> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _users.ExistsAsync(request.Name, cancellationToken))
        {
            return null;
        }

        var user = new User(request.Name, _passwordHasher.Hash(request.Password));
        await _users.AddAsync(user, cancellationToken);

        return user.Id;
    }
}
