using MediatR;
using UserService.Application.Abstractions;
using UserService.Domain.Entities;

namespace UserService.Application.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IRevokedTokenRepository _revokedTokens;

    public LogoutCommandHandler(IRevokedTokenRepository revokedTokens)
    {
        _revokedTokens = revokedTokens;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _revokedTokens.AddAsync(new RevokedToken(request.Jti, request.ExpiresAt), cancellationToken);
    }
}
