using FinanceService.Application.Abstractions;
using FinanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not set");

        services.AddDbContext<FinanceDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IFavoriteCurrencyRepository, FavoriteCurrencyRepository>();
        services.AddScoped<IRevokedTokenRepository, RevokedTokenRepository>();

        return services;
    }
}
