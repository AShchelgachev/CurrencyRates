namespace UserService.Domain.Entities;

public class User
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    public string PasswordHash { get; private set; }

    public User(string name, string passwordHash)
    {
        Name = name;
        PasswordHash = passwordHash;
    }
}
