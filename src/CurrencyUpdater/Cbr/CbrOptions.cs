namespace CurrencyUpdater.Cbr;

public class CbrOptions
{
    public const string SectionName = "Cbr";

    public string Url { get; set; } = null!;

    public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromHours(1);
}
