using UserService.Domain.Entities;

namespace UserService.Application.Abstractions;

public interface IUserRepository
{
    Task<bool> ExistsAsync(string name, CancellationToken cancellationToken);

    Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken);

    Task AddAsync(User user, CancellationToken cancellationToken);
}
