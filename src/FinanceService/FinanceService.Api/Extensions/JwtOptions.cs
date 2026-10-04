namespace FinanceService.Api.Extensions;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = null!;

    public string Audience { get; set; } = null!;

    public string SigningKey { get; set; } = null!;
}
