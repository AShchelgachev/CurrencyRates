using System.Security.Claims;
using System.Text;
using FinanceService.Application.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FinanceService.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("Section 'Jwt' is not set");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var jti = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Jti);
                        var revokedTokens = context.HttpContext.RequestServices.GetRequiredService<IRevokedTokenRepository>();

                        if (jti is null || await revokedTokens.IsRevokedAsync(jti, context.HttpContext.RequestAborted))
                        {
                            context.Fail("Token has been revoked");
                        }
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}
