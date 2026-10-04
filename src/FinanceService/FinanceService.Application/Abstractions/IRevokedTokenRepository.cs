namespace FinanceService.Application.Abstractions;

public interface IRevokedTokenRepository
{
    Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken);
}
