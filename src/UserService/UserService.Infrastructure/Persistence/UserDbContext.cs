using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");
            entity.Property(x => x.PasswordHash).HasColumnName("password");
        });

        modelBuilder.Entity<RevokedToken>(entity =>
        {
            entity.ToTable("revoked_token");
            entity.HasKey(x => x.Jti);
        });
    }
}
