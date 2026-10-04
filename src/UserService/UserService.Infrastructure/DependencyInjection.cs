using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Abstractions;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Security;

namespace UserService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not set");

        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRevokedTokenRepository, RevokedTokenRepository>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
