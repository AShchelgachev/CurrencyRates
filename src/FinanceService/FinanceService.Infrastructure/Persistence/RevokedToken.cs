namespace FinanceService.Infrastructure.Persistence;

public class RevokedToken
{
    public string Jti { get; private set; }

    public RevokedToken(string jti)
    {
        Jti = jti;
    }
}
