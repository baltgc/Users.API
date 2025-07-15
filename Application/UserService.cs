using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Users.API.Domain;
using Users.API.Enums;
using Users.API.Infrastructure;

namespace Users.API.Application;

public class UserService
{
    private readonly UserDbContext _db;
    private readonly JwtTokenGenerator _jwt;

    public UserService(UserDbContext db, JwtTokenGenerator jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task RegisterAsync(string email, string password, UserRole role)
    {
        if (await _db.Users.AnyAsync(u => u.Email == email))
            throw new Exception("Email ya registrado");

        var user = new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role,
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user =
            await _db.Users.FirstOrDefaultAsync(u => u.Email == email)
            ?? throw new Exception("Usuario no encontrado");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new Exception("Contraseña incorrecta");

        return _jwt.GenerateToken(user.Email, user.Role.ToString(), user.Id);
    }

    public async Task<User> GetCurrentUserAsync(ClaimsPrincipal userClaims)
    {
        var email =
            userClaims.FindFirstValue(ClaimTypes.Email)
            ?? throw new Exception("No se encontró el email en el token");

        var user =
            await _db.Users.FirstOrDefaultAsync(u => u.Email == email)
            ?? throw new Exception("Usuario no encontrado");

        return user;
    }
}
