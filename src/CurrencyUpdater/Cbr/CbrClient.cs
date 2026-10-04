using System.Globalization;
using System.Xml.Linq;
using Microsoft.Extensions.Options;

namespace CurrencyUpdater.Cbr;

public class CbrClient
{
    private static readonly NumberFormatInfo CbrNumberFormat = new() { NumberDecimalSeparator = "," };

    private readonly HttpClient _httpClient;
    private readonly CbrOptions _options;

    public CbrClient(HttpClient httpClient, IOptions<CbrOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<CbrRate>> GetDailyRatesAsync(CancellationToken cancellationToken)
    {
        var xml = await _httpClient.GetStringAsync(_options.Url, cancellationToken);
        var document = XDocument.Parse(xml);

        return document.Root!
            .Elements("Valute")
            .Select(valute => new CbrRate(
                valute.Element("CharCode")!.Value,
                decimal.Parse(valute.Element("VunitRate")!.Value, NumberStyles.Float, CbrNumberFormat)))
            .ToList();
    }
}
