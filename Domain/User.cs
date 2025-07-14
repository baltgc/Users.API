using Users.API.Enums;

namespace Users.API.Domain;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public UserRole Role { get; set; }
}
