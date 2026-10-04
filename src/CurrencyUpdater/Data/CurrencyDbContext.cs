using Microsoft.EntityFrameworkCore;

namespace CurrencyUpdater.Data;

public class CurrencyDbContext : DbContext
{
    public CurrencyDbContext(DbContextOptions<CurrencyDbContext> options) : base(options)
    {
    }

    public DbSet<Currency> Currencies => Set<Currency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>().ToTable("currency");
    }
}
