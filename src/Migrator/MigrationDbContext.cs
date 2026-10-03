using Microsoft.EntityFrameworkCore;
using Migrator.Entities;

namespace Migrator;

public class MigrationDbContext : DbContext
{
    public MigrationDbContext(DbContextOptions<MigrationDbContext> options) : base(options)
    {
    }

    public DbSet<Currency> Currencies => Set<Currency>();

    public DbSet<User> Users => Set<User>();

    public DbSet<UserFavoriteCurrency> UserFavoriteCurrencies => Set<UserFavoriteCurrency>();

    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>(entity =>
        {
            entity.ToTable("currency");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Rate).HasColumnType("numeric");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<UserFavoriteCurrency>(entity =>
        {
            entity.ToTable("user_favorite_currency");
            entity.HasKey(x => new { x.UserId, x.CurrencyId });

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Currency>()
                .WithMany()
                .HasForeignKey(x => x.CurrencyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RevokedToken>(entity =>
        {
            entity.ToTable("revoked_token");
            entity.HasKey(x => x.Jti);
        });
    }
}
