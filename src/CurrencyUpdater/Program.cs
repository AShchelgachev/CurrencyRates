using System.Text;
using CurrencyUpdater;
using CurrencyUpdater.Cbr;
using CurrencyUpdater.Data;
using Microsoft.EntityFrameworkCore;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' is not set");

builder.Services.Configure<CbrOptions>(builder.Configuration.GetSection(CbrOptions.SectionName));

builder.Services.AddDbContext<CurrencyDbContext>(options =>
    options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

builder.Services.AddHttpClient<CbrClient>(client =>
    client.DefaultRequestHeaders.UserAgent.ParseAdd("CurrencyRates/1.0"));

builder.Services.AddHostedService<CurrencyUpdateWorker>();

var host = builder.Build();
host.Run();
