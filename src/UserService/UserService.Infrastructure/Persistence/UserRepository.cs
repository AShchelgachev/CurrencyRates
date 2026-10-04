using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstractions;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _db;

    public UserRepository(UserDbContext db)
    {
        _db = db;
    }

    public Task<bool> ExistsAsync(string name, CancellationToken cancellationToken) =>
        _db.Users.AnyAsync(u => u.Name == name, cancellationToken);

    public Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken) =>
        _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Name == name, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
