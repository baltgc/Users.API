using Microsoft.EntityFrameworkCore;
using Users.API.Domain;

namespace Users.API.Infrastructure;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
}
