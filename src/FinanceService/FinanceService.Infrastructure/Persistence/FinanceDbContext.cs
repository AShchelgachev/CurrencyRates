using FinanceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Persistence;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
    {
    }

    public DbSet<Currency> Currencies => Set<Currency>();

    public DbSet<UserFavoriteCurrency> UserFavoriteCurrencies => Set<UserFavoriteCurrency>();

    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>().ToTable("currency");

        modelBuilder.Entity<UserFavoriteCurrency>(entity =>
        {
            entity.ToTable("user_favorite_currency");
            entity.HasKey(x => new { x.UserId, x.CurrencyId });
        });

        modelBuilder.Entity<RevokedToken>(entity =>
        {
            entity.ToTable("revoked_token");
            entity.HasKey(x => x.Jti);
        });
    }
}
