namespace Migrator.Entities;

public class RevokedToken
{
    public string Jti { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }
}
