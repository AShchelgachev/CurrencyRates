using CurrencyUpdater.Cbr;
using CurrencyUpdater.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CurrencyUpdater;

public class CurrencyUpdateWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly CbrOptions _options;
    private readonly ILogger<CurrencyUpdateWorker> _logger;

    public CurrencyUpdateWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<CbrOptions> options,
        ILogger<CurrencyUpdateWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_options.UpdateInterval);

        do
        {
            try
            {
                await UpdateRatesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update currency rates");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task UpdateRatesAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var cbrClient = scope.ServiceProvider.GetRequiredService<CbrClient>();
        var db = scope.ServiceProvider.GetRequiredService<CurrencyDbContext>();

        var rates = await cbrClient.GetDailyRatesAsync(cancellationToken);
        var currencies = await db.Currencies.ToDictionaryAsync(c => c.Name, cancellationToken);

        foreach (var rate in rates)
        {
            if (currencies.TryGetValue(rate.Code, out var currency))
            {
                currency.Rate = rate.Rate;
            }
            else
            {
                db.Currencies.Add(new Currency { Name = rate.Code, Rate = rate.Rate });
            }
        }

        await db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Currency rates updated: {Count} currencies", rates.Count);
    }
}
