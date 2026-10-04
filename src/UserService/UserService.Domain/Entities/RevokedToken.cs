namespace UserService.Domain.Entities;

public class RevokedToken
{
    public string Jti { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public RevokedToken(string jti, DateTime expiresAt)
    {
        Jti = jti;
        ExpiresAt = expiresAt;
    }
}
