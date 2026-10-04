using UserService.Domain.Entities;

namespace UserService.Application.Abstractions;

public interface IRevokedTokenRepository
{
    Task AddAsync(RevokedToken token, CancellationToken cancellationToken);

    Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken);
}
